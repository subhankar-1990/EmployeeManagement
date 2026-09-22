import { ChangeDetectionStrategy, Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, Subject, catchError, debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs';
import {
  DEFAULT_PAGE_NUMBER,
  DEFAULT_PAGE_SIZE,
  EmployeeResponse,
  PagedResponseOfEmployeeResponse,
  SEARCH_DEBOUNCE_MS,
} from '../../../core/models/employee.model';
import type { EmployeeListQuery } from '../../../core/models/employee.model';
import { EmployeeService } from '../../../core/services/employee.service';
import { errorMessageOf } from '../../../core/utils/api-error';
import { ConfirmDialog } from '../../../shared/components/confirm-dialog/confirm-dialog';
import { Pagination } from '../../../shared/components/pagination/pagination';

/**
 * `GET /api/v1/employees` backed directory.
 *
 * The screen keeps its paging and search state in signals and drives the API through a single
 * `switchMap` pipeline: typing in the search box or paging pushes the next request in, so a slow
 * response can never overwrite the results of a newer one.
 */
@Component({
  selector: 'app-employee-list',
  imports: [RouterLink, Pagination, ConfirmDialog],
  templateUrl: './employee-list.html',
  styleUrl: './employee-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmployeeList {
  private readonly employeeService = inject(EmployeeService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  /** Raw keystrokes, debounced before they reach the API. */
  private readonly searchInput = new Subject<string>();

  /** Every value pushed here issues one request. */
  private readonly reloads = new Subject<void>();

  protected readonly employees = signal<readonly EmployeeResponse[]>([]);
  protected readonly searchTerm = signal('');
  protected readonly pageNumber = signal(DEFAULT_PAGE_NUMBER);
  protected readonly pageSize = signal(DEFAULT_PAGE_SIZE);
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected readonly hasPreviousPage = signal(false);
  protected readonly hasNextPage = signal(false);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly notice = signal<string | null>(null);
  protected readonly pendingDelete = signal<EmployeeResponse | null>(null);
  protected readonly deleteBusy = signal(false);

  protected readonly showEmptyState = computed(
    () => !this.loading() && this.errorMessage() === null && this.employees().length === 0,
  );

  /** Prompt shown by the delete confirmation dialog. */
  protected readonly deleteMessage = computed(() => {
    const employee = this.pendingDelete();

    return `Delete "${employee?.name ?? 'this employee'}" permanently? This cannot be undone.`;
  });

  constructor() {
    // Flash messages handed over by the create and edit screens.
    this.route.queryParamMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      if (params.has('created')) {
        this.notice.set('The employee was created.');
      } else if (params.has('updated')) {
        this.notice.set('The employee was updated.');
      } else if (params.has('deleted')) {
        this.notice.set('The employee was deleted.');
      }
    });

    this.searchInput
      .pipe(debounceTime(SEARCH_DEBOUNCE_MS), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((term) => {
        this.searchTerm.set(term);
        this.pageNumber.set(DEFAULT_PAGE_NUMBER);
        this.reload();
      });

    this.reloads
      .pipe(
        tap(() => {
          this.loading.set(true);
          this.errorMessage.set(null);
        }),
        // `catchError` inside `switchMap` keeps the outer pipeline alive after a failure.
        switchMap(() =>
          this.employeeService.getEmployees(this.currentQuery()).pipe(
            catchError((error: unknown) => {
              this.applyLoadFailure(error);
              return EMPTY;
            }),
          ),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((page) => this.applyPage(page));

    this.reload();
  }

  /** Loads the current page again. */
  protected reload(): void {
    this.reloads.next();
  }

  protected onSearchInput(event: Event): void {
    this.searchInput.next((event.target as HTMLInputElement).value);
  }

  protected onPageChange(page: number): void {
    this.pageNumber.set(page);
    this.reload();
  }

  protected onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.pageNumber.set(DEFAULT_PAGE_NUMBER);
    this.reload();
  }

  protected dismissNotice(): void {
    this.notice.set(null);
    // Drop the `created` / `updated` marker so a refresh does not replay the message.
    void this.router.navigate([], { relativeTo: this.route, queryParams: {}, replaceUrl: true });
  }

  protected requestDelete(employee: EmployeeResponse): void {
    this.pendingDelete.set(employee);
  }

  protected cancelDelete(): void {
    if (!this.deleteBusy()) {
      this.pendingDelete.set(null);
    }
  }

  protected confirmDelete(): void {
    const employee = this.pendingDelete();
    if (!employee || employee.id === undefined) {
      return;
    }

    this.deleteBusy.set(true);

    this.employeeService
      .deleteEmployee(employee.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.deleteBusy.set(false);
          this.pendingDelete.set(null);
          this.notice.set(`"${employee.name ?? 'Employee'}" was deleted.`);
          this.afterDelete();
        },
        error: (error: unknown) => {
          this.deleteBusy.set(false);
          this.pendingDelete.set(null);
          this.errorMessage.set(errorMessageOf(error));
        },
      });
  }

  private afterDelete(): void {
    // Deleting the last row of a page would leave the user on an empty page.
    if (this.employees().length <= 1 && this.pageNumber() > DEFAULT_PAGE_NUMBER) {
      this.pageNumber.update((page) => page - 1);
    }

    this.reload();
  }

  private currentQuery(): EmployeeListQuery {
    const search = this.searchTerm().trim();

    return {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      search: search.length > 0 ? search : undefined,
    };
  }

  private applyPage(page: PagedResponseOfEmployeeResponse): void {
    this.employees.set(page.items ?? []);
    this.totalCount.set(page.totalCount ?? 0);
    this.totalPages.set(page.totalPages ?? 0);
    this.hasPreviousPage.set(page.hasPreviousPage ?? false);
    this.hasNextPage.set(page.hasNextPage ?? false);
    this.loading.set(false);
  }

  private applyLoadFailure(error: unknown): void {
    this.employees.set([]);
    this.totalCount.set(0);
    this.totalPages.set(0);
    this.hasPreviousPage.set(false);
    this.hasNextPage.set(false);
    this.loading.set(false);
    this.errorMessage.set(errorMessageOf(error));
  }
}
