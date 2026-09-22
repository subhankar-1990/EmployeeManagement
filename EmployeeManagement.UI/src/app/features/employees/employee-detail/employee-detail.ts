import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EmployeeResponse } from '../../../core/models/employee.model';
import { EmployeeService } from '../../../core/services/employee.service';
import { errorMessageOf } from '../../../core/utils/api-error';
import { ConfirmDialog } from '../../../shared/components/confirm-dialog/confirm-dialog';

/**
 * Read-only view of a single employee (`GET /api/v1/employees/{id}`), with the entry points to the
 * edit screen and to the delete endpoint.
 */
@Component({
  selector: 'app-employee-detail',
  imports: [RouterLink, ConfirmDialog],
  templateUrl: './employee-detail.html',
  styleUrl: './employee-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmployeeDetail {
  private readonly employeeService = inject(EmployeeService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly employee = signal<EmployeeResponse | null>(null);
  protected readonly employeeId = signal<number | null>(null);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly deleteRequested = signal(false);
  protected readonly deleteBusy = signal(false);

  /** Prompt shown by the delete confirmation dialog. */
  protected readonly deleteMessage = computed(() => {
    const employee = this.employee();

    return `Delete "${employee?.name ?? 'this employee'}" permanently? This cannot be undone.`;
  });

  constructor() {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      const id = Number(params.get('id'));

      if (!Number.isInteger(id) || id <= 0) {
        this.employeeId.set(null);
        this.employee.set(null);
        this.errorMessage.set('The employee id in the address is not valid.');
        return;
      }

      this.employeeId.set(id);
      this.load(id);
    });
  }

  /** Re-requests the employee currently displayed. */
  protected reload(): void {
    const id = this.employeeId();
    if (id !== null) {
      this.load(id);
    }
  }

  protected requestDelete(): void {
    this.deleteRequested.set(true);
  }

  protected cancelDelete(): void {
    if (!this.deleteBusy()) {
      this.deleteRequested.set(false);
    }
  }

  protected confirmDelete(): void {
    const id = this.employeeId();
    if (id === null) {
      return;
    }

    this.deleteBusy.set(true);

    this.employeeService
      .deleteEmployee(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.deleteBusy.set(false);
          this.deleteRequested.set(false);
          void this.router.navigate(['/employees'], { queryParams: { deleted: 1 } });
        },
        error: (error: unknown) => {
          this.deleteBusy.set(false);
          this.deleteRequested.set(false);
          this.errorMessage.set(errorMessageOf(error));
        },
      });
  }

  private load(id: number): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.employeeService
      .getEmployeeById(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (employee) => {
          this.employee.set(employee);
          this.loading.set(false);
        },
        error: (error: unknown) => {
          this.employee.set(null);
          this.errorMessage.set(errorMessageOf(error));
          this.loading.set(false);
        },
      });
  }
}
