import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import type { Mock } from 'vitest';
import {
  CreateEmployeeRequest,
  EmployeeResponse,
  EmployeesClient,
  PagedResponseOfEmployeeResponse,
  ProblemDetails,
  UpdateEmployeeRequest,
} from '../models/employee.model';
import { ApiError } from '../utils/api-error';
import { EmployeeService } from './employee.service';

interface EmployeesClientStub {
  getEmployees: Mock;
  getEmployeeById: Mock;
  createEmployee: Mock;
  updateEmployee: Mock;
  deleteEmployee: Mock;
}

/** The generated client is swapped for a stub: no HTTP and no server are needed here. */
function createClientStub(): EmployeesClientStub {
  return {
    getEmployees: vi.fn(),
    getEmployeeById: vi.fn(),
    createEmployee: vi.fn(),
    updateEmployee: vi.fn(),
    deleteEmployee: vi.fn(),
  };
}

describe('EmployeeService', () => {
  let client: EmployeesClientStub;
  let service: EmployeeService;

  beforeEach(() => {
    client = createClientStub();

    TestBed.configureTestingModule({
      providers: [
        EmployeeService,
        { provide: EmployeesClient, useValue: client as unknown as EmployeesClient },
      ],
    });

    service = TestBed.inject(EmployeeService);
  });

  describe('getEmployees', () => {
    it('forwards the page and the trimmed search term', async () => {
      const page = new PagedResponseOfEmployeeResponse({ items: [], pageNumber: 2, pageSize: 20, totalCount: 0, totalPages: 0 });
      client.getEmployees.mockResolvedValue(page);

      const result = await firstValueFrom(service.getEmployees({ pageNumber: 2, pageSize: 20, search: '  ada  ' }));

      expect(client.getEmployees).toHaveBeenCalledWith(2, 20, 'ada');
      expect(result).toBe(page);
    });

    it('omits a search term that contains only whitespace', async () => {
      client.getEmployees.mockResolvedValue(new PagedResponseOfEmployeeResponse());

      await firstValueFrom(service.getEmployees({ pageNumber: 1, pageSize: 10, search: '   ' }));

      expect(client.getEmployees).toHaveBeenCalledWith(1, 10, undefined);
    });

    it('normalises a validation ProblemDetails into an ApiError', async () => {
      const problem = new ProblemDetails({ status: 400, title: 'One or more validation errors occurred.' });
      problem['errors'] = { PageSize: ['Page size must be between 1 and 100.'] };
      client.getEmployees.mockRejectedValue(problem);

      const error = (await firstValueFrom(service.getEmployees({ pageNumber: 1, pageSize: 500 })).catch(
        (caught: unknown) => caught,
      )) as ApiError;

      expect(error).toBeInstanceOf(ApiError);
      expect(error.status).toBe(400);
      expect(error.hasFieldErrors).toBe(true);
      expect(error.fieldErrors['PageSize']).toEqual(['Page size must be between 1 and 100.']);
      expect(error.message).toBe('One or more validation errors occurred.');
    });
  });

  describe('getEmployeeById', () => {
    it('describes a missing employee in plain language', async () => {
      client.getEmployeeById.mockRejectedValue(new ProblemDetails({ status: 404, title: 'Not Found' }));

      const error = (await firstValueFrom(service.getEmployeeById(99)).catch(
        (caught: unknown) => caught,
      )) as ApiError;

      expect(error.status).toBe(404);
      expect(error.message).toContain('could not be found');
    });
  });

  describe('createEmployee', () => {
    it('passes the supplied idempotency key and the request body through', async () => {
      const created = new EmployeeResponse({ id: 7, name: 'Ada Lovelace' });
      client.createEmployee.mockResolvedValue(created);
      const request = new CreateEmployeeRequest({
        name: 'Ada Lovelace',
        mobile: '+441234567890',
        email: 'ada@example.com',
        isActive: true,
      });

      const result = await firstValueFrom(service.createEmployee(request, 'key-1'));

      expect(client.createEmployee).toHaveBeenCalledWith('key-1', request);
      expect(result).toBe(created);
    });

    it('generates an idempotency key when the caller omits one', async () => {
      client.createEmployee.mockResolvedValue(new EmployeeResponse());

      await firstValueFrom(service.createEmployee(new CreateEmployeeRequest({ name: 'Ada Lovelace' })));

      const call = client.createEmployee.mock.calls[0] as [string, CreateEmployeeRequest];
      expect(typeof call[0]).toBe('string');
      expect(call[0].length).toBeGreaterThan(0);
    });
  });

  describe('updateEmployee', () => {
    it('completes with undefined for the 204 response', async () => {
      client.updateEmployee.mockResolvedValue(undefined);
      const request = new UpdateEmployeeRequest({ name: 'Ada Lovelace' });

      await expect(firstValueFrom(service.updateEmployee(5, request))).resolves.toBeUndefined();
      expect(client.updateEmployee).toHaveBeenCalledWith(5, request);
    });
  });

  describe('deleteEmployee', () => {
    it('explains a network failure instead of leaking the TypeError', async () => {
      client.deleteEmployee.mockRejectedValue(new TypeError('Failed to fetch'));

      const error = (await firstValueFrom(service.deleteEmployee(3)).catch(
        (caught: unknown) => caught,
      )) as ApiError;

      expect(error.status).toBeNull();
      expect(error.message).toContain('could not be reached');
    });
  });
});
