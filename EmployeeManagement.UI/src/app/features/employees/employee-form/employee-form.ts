import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable, map } from 'rxjs';
import {
  CreateEmployeeRequest,
  EMPLOYEE_LIMITS,
  EmployeeResponse,
  UpdateEmployeeRequest,
} from '../../../core/models/employee.model';
import type { EmployeeFormValue } from '../../../core/models/employee.model';
import { EmployeeService } from '../../../core/services/employee.service';
import { errorMessageOf, fieldErrorsOf } from '../../../core/utils/api-error';

/** Which endpoint the form will call when it is submitted. */
type FormMode = 'create' | 'edit';

/** Result handed back to the list screen through a query parameter. */
type SaveOutcome = 'created' | 'updated';

/**
 * Create and edit screen for a single employee.
 *
 * One component serves both routes (`/employees/new` and `/employees/:id/edit`) because the payload
 * and the validation rules are identical; only the endpoint and the initial values differ. Client
 * side rules mirror `CreateEmployeeCommandValidator` so the user gets the same feedback the API
 * would return, and any server side `errors` dictionary is mapped back onto the matching control.
 */
@Component({
  selector: 'app-employee-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './employee-form.html',
  styleUrl: './employee-form.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmployeeForm {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly employeeService = inject(EmployeeService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  /** Exposed to the template so the inputs advertise the same ceilings the API enforces. */
  protected readonly limits = EMPLOYEE_LIMITS;

  protected readonly form = this.formBuilder.group({
    name: this.formBuilder.control('', [Validators.required, Validators.maxLength(EMPLOYEE_LIMITS.name)]),
    mobile: this.formBuilder.control('', [Validators.required, Validators.maxLength(EMPLOYEE_LIMITS.mobile)]),
    email: this.formBuilder.control('', [
      Validators.required,
      Validators.email,
      Validators.maxLength(EMPLOYEE_LIMITS.email),
    ]),
    isActive: this.formBuilder.control(true),
  });

  protected readonly mode = signal<FormMode>('create');
  protected readonly employeeId = signal<number | null>(null);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly saveError = signal<string | null>(null);
  protected readonly isEditMode = computed(() => this.mode() === 'edit');

  constructor() {
    // Re-initialises the screen when the id segment changes without recreating the component.
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      this.initialize(params.get('id'));
    });
  }

  /** Loads (or reloads) the employee being edited. */
  protected loadEmployee(): void {
    const id = this.employeeId();
    if (id === null) {
      return;
    }

    this.loading.set(true);
    this.loadError.set(null);

    this.employeeService
      .getEmployeeById(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (employee) => {
          this.patchForm(employee);
          this.loading.set(false);
        },
        error: (error: unknown) => {
          this.loadError.set(errorMessageOf(error));
          this.loading.set(false);
        },
      });
  }

  /** Validates, then posts the employee to the create or the update endpoint. */
  protected submit(): void {
    this.saveError.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const payload: EmployeeFormValue = {
      name: value.name.trim(),
      mobile: value.mobile.trim(),
      email: value.email.trim(),
      isActive: value.isActive,
    };

    this.saving.set(true);

    this.saveRequest(payload)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (outcome) => {
          this.saving.set(false);
          void this.router.navigate(['/employees'], { queryParams: { [outcome]: 1 } });
        },
        error: (error: unknown) => {
          this.saving.set(false);
          this.applyServerErrors(error);
        },
      });
  }

  /** Leaves the form, returning to the employee's detail page when editing. */
  protected cancel(): void {
    const id = this.employeeId();

    if (this.mode() === 'edit' && id !== null) {
      void this.router.navigate(['/employees', id]);
      return;
    }

    void this.router.navigate(['/employees']);
  }

  /** True when a control should surface its validation message. */
  protected showError(controlName: string): boolean {
    const control = this.form.get(controlName);

    return control !== null && control.invalid && (control.touched || control.dirty);
  }

  /** Message for the first failing rule of a control, matching the wording used by the API. */
  protected validationMessage(controlName: string, label: string): string {
    const errors = this.form.get(controlName)?.errors;

    if (!errors) {
      return '';
    }

    if (typeof errors['server'] === 'string') {
      return errors['server'];
    }

    if (errors['required']) {
      return `${label} is required.`;
    }

    if (errors['email']) {
      return `${label} must be a valid email address.`;
    }

    if (errors['maxlength']) {
      const { requiredLength } = errors['maxlength'] as { requiredLength: number };
      return `${label} must not exceed ${requiredLength} characters.`;
    }

    return `${label} is not valid.`;
  }

  private initialize(idParam: string | null): void {
    const id = idParam ? Number(idParam) : Number.NaN;

    if (Number.isInteger(id) && id > 0) {
      this.mode.set('edit');
      this.employeeId.set(id);
      this.loadEmployee();
      return;
    }

    this.mode.set('create');
    this.employeeId.set(null);
    this.loadError.set(null);
    this.loading.set(false);
    this.form.reset({ name: '', mobile: '', email: '', isActive: true });
  }

  private patchForm(employee: EmployeeResponse): void {
    this.form.reset({
      name: employee.name ?? '',
      mobile: employee.mobile ?? '',
      email: employee.email ?? '',
      isActive: employee.isActive ?? true,
    });
  }

  private saveRequest(payload: EmployeeFormValue): Observable<SaveOutcome> {
    const id = this.employeeId();

    if (this.mode() === 'edit' && id !== null) {
      return this.employeeService
        .updateEmployee(id, new UpdateEmployeeRequest(payload))
        .pipe(map(() => 'updated' as const));
    }

    // A fresh idempotency key per submission: a retry after a network blip cannot insert twice.
    return this.employeeService.createEmployee(new CreateEmployeeRequest(payload)).pipe(map(() => 'created' as const));
  }

  private applyServerErrors(error: unknown): void {
    this.saveError.set(errorMessageOf(error));

    for (const [field, messages] of Object.entries(fieldErrorsOf(error))) {
      const control = this.form.get(this.controlNameFor(field));

      if (control && messages.length > 0) {
        control.setErrors({ server: messages[0] });
        control.markAsTouched();
      }
    }
  }

  /** The API reports `Name`, the form control is `name`. */
  private controlNameFor(field: string): string {
    const camelCased = field.charAt(0).toLowerCase() + field.slice(1);

    return this.form.contains(camelCased) ? camelCased : field.toLowerCase();
  }
}
