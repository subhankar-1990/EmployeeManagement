import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router, convertToParamMap, provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import type { Mock } from 'vitest';
import { CreateEmployeeRequest, EmployeeResponse, UpdateEmployeeRequest } from '../../../core/models/employee.model';
import { EmployeeService } from '../../../core/services/employee.service';
import { ApiError } from '../../../core/utils/api-error';
import { EmployeeForm } from './employee-form';

interface EmployeeServiceStub {
  getEmployees: Mock;
  getEmployeeById: Mock;
  createEmployee: Mock;
  updateEmployee: Mock;
  deleteEmployee: Mock;
}

function createServiceStub(): EmployeeServiceStub {
  return {
    getEmployees: vi.fn(),
    getEmployeeById: vi.fn(),
    createEmployee: vi.fn(),
    updateEmployee: vi.fn(),
    deleteEmployee: vi.fn(),
  };
}

function rootOf(fixture: ComponentFixture<EmployeeForm>): HTMLElement {
  return fixture.nativeElement as HTMLElement;
}

function setInput(fixture: ComponentFixture<EmployeeForm>, selector: string, value: string): void {
  const control = rootOf(fixture).querySelector(selector) as HTMLInputElement | null;
  if (!control) {
    throw new Error(`No element matched "${selector}".`);
  }

  control.value = value;
  control.dispatchEvent(new Event('input'));
}

function submit(fixture: ComponentFixture<EmployeeForm>): void {
  const form = rootOf(fixture).querySelector('form') as HTMLFormElement | null;
  if (!form) {
    throw new Error('The form was not rendered.');
  }

  form.dispatchEvent(new Event('submit'));
  fixture.detectChanges();
}

function textOf(fixture: ComponentFixture<EmployeeForm>, selector: string): string {
  return rootOf(fixture).querySelector(selector)?.textContent?.trim() ?? '';
}

function loadGrace(): EmployeeResponse {
  return new EmployeeResponse({
    id: 5,
    name: 'Grace Hopper',
    mobile: '+15550001',
    email: 'grace@example.com',
    isActive: false,
  });
}

function loadFailed(): ApiError {
  return new ApiError('The employee could not be found. It may have been deleted by someone else.', 404);
}

describe('EmployeeForm', () => {
  let service: EmployeeServiceStub;

  async function setup(
    params: Record<string, string> = {},
    configure?: (stub: EmployeeServiceStub) => void,
  ): Promise<ComponentFixture<EmployeeForm>> {
    service = createServiceStub();

    await TestBed.configureTestingModule({
      imports: [EmployeeForm],
      providers: [
        provideRouter([]),
        { provide: EmployeeService, useValue: service as unknown as EmployeeService },
        { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap(params)) } },
      ],
    }).compileComponents();

    // Runs before the component is created so edit-mode tests can stub the load call in time.
    configure?.(service);

    const fixture = TestBed.createComponent(EmployeeForm);
    await fixture.whenStable();
    fixture.detectChanges();

    return fixture;
  }

  beforeEach(() => {
    TestBed.resetTestingModule();
  });

  it('renders an empty form on /employees/new', async () => {
    const fixture = await setup();

    expect(textOf(fixture, '.page__title')).toBe('Add employee');
    expect((rootOf(fixture).querySelector('#name') as HTMLInputElement | null)?.value).toBe('');
    expect(textOf(fixture, 'button[type="submit"]')).toBe('Create employee');
  });

  it('refuses to submit an empty form', async () => {
    const fixture = await setup();

    submit(fixture);

    expect(service.createEmployee).not.toHaveBeenCalled();
    const errors = Array.from(rootOf(fixture).querySelectorAll('.field__error')).map(
      (element: Element) => element.textContent?.trim(),
    );

    expect(errors).toContain('Name is required.');
    expect(errors).toContain('Mobile is required.');
    expect(errors).toContain('Email is required.');
  });

  it('trims the entered values and navigates to the list after a successful create', async () => {
    const fixture = await setup({}, (stub) =>
      stub.createEmployee.mockReturnValue(of(new EmployeeResponse({ id: 12 }))),
    );
    const navigate = vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);

    setInput(fixture, '#name', '  Ada Lovelace  ');
    setInput(fixture, '#mobile', ' +441234567890 ');
    setInput(fixture, '#email', ' ada@example.com ');
    submit(fixture);

    expect(service.createEmployee).toHaveBeenCalledTimes(1);
    const [request] = service.createEmployee.mock.calls[0] as [CreateEmployeeRequest];
    expect(request).toBeInstanceOf(CreateEmployeeRequest);
    expect(request.name).toBe('Ada Lovelace');
    expect(request.mobile).toBe('+441234567890');
    expect(request.email).toBe('ada@example.com');
    expect(request.isActive).toBe(true);
    expect(navigate).toHaveBeenCalledWith(['/employees'], { queryParams: { created: 1 } });
  });

  it('rejects an email address the API would reject as well', async () => {
    const fixture = await setup();

    setInput(fixture, '#name', 'Ada Lovelace');
    setInput(fixture, '#mobile', '+441234567890');
    setInput(fixture, '#email', 'not-an-email');
    submit(fixture);

    expect(service.createEmployee).not.toHaveBeenCalled();
    expect(textOf(fixture, '.field__error')).toBe('Email must be a valid email address.');
  });

  it('shows the server side validation message against the offending field', async () => {
    const serverError = () =>
      new ApiError('One or more validation errors occurred.', 400, {
        Name: ['Name is already in use.'],
      });
    const fixture = await setup({}, (stub) => stub.createEmployee.mockReturnValue(throwError(serverError)));

    setInput(fixture, '#name', 'Ada Lovelace');
    setInput(fixture, '#mobile', '+441234567890');
    setInput(fixture, '#email', 'ada@example.com');
    submit(fixture);

    expect(textOf(fixture, '.alert--error .alert__message')).toBe('One or more validation errors occurred.');
    expect(textOf(fixture, '.field__error')).toBe('Name is already in use.');
  });

  it('loads the employee when the route carries an id', async () => {
    const fixture = await setup({ id: '5' }, (stub) => stub.getEmployeeById.mockReturnValue(of(loadGrace())));

    expect(service.getEmployeeById).toHaveBeenCalledWith(5);
    expect(textOf(fixture, '.page__title')).toBe('Edit employee');
    expect((rootOf(fixture).querySelector('#name') as HTMLInputElement | null)?.value).toBe('Grace Hopper');
    expect((rootOf(fixture).querySelector('#email') as HTMLInputElement | null)?.value).toBe('grace@example.com');
    expect((rootOf(fixture).querySelector('#isActive') as HTMLInputElement | null)?.checked).toBe(false);
  });

  it('saves through the update endpoint when editing', async () => {
    const fixture = await setup({ id: '5' }, (stub) => {
      stub.getEmployeeById.mockReturnValue(of(loadGrace()));
      stub.updateEmployee.mockReturnValue(of(undefined));
    });
    const navigate = vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);

    setInput(fixture, '#name', 'Rear Admiral Grace Hopper');
    submit(fixture);

    expect(service.createEmployee).not.toHaveBeenCalled();
    const [id, request] = service.updateEmployee.mock.calls[0] as [number, UpdateEmployeeRequest];
    expect(id).toBe(5);
    expect(request).toBeInstanceOf(UpdateEmployeeRequest);
    expect(request.name).toBe('Rear Admiral Grace Hopper');
    expect(navigate).toHaveBeenCalledWith(['/employees'], { queryParams: { updated: 1 } });
  });

  it('offers a retry when the employee cannot be loaded', async () => {
    const fixture = await setup({ id: '5' }, (stub) => stub.getEmployeeById.mockReturnValue(throwError(() => loadFailed())));

    expect(textOf(fixture, '.alert--error .alert__message')).toContain('could not be found');
    expect(textOf(fixture, 'button')).toBe('Try again');
    expect(rootOf(fixture).querySelector('form')).toBeNull();
  });
});
