import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { PAGE_SIZE_OPTIONS } from '../../../core/models/employee.model';

/**
 * Paging controls for a collection endpoint.
 *
 * Presentational only: it renders the state handed to it and emits the page or page size the user
 * asked for. Rendering at most five page buttons keeps the control stable for any `totalPages`.
 */
@Component({
  selector: 'app-pagination',
  imports: [],
  templateUrl: './pagination.html',
  styleUrl: './pagination.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Pagination {
  /** One based page that is currently displayed. */
  readonly pageNumber = input.required<number>();

  /** Rows per page that is currently applied. */
  readonly pageSize = input.required<number>();

  /** Total number of rows matching the current filter. */
  readonly totalCount = input.required<number>();

  /** Total number of pages reported by the API. */
  readonly totalPages = input.required<number>();

  readonly hasPreviousPage = input.required<boolean>();
  readonly hasNextPage = input.required<boolean>();

  /** Disables every control, e.g. while a request is in flight. */
  readonly disabled = input(false);

  /** Numbers offered by the rows-per-page select. */
  readonly pageSizeOptions = input<readonly number[]>(PAGE_SIZE_OPTIONS);

  readonly pageChange = output<number>();
  readonly pageSizeChange = output<number>();

  /** 1 based number of the first row on the current page, or 0 when the collection is empty. */
  protected readonly firstRowNumber = computed(() =>
    this.totalCount() === 0 ? 0 : (this.pageNumber() - 1) * this.pageSize() + 1,
  );

  /** 1 based number of the last row on the current page. */
  protected readonly lastRowNumber = computed(() =>
    Math.min(this.pageNumber() * this.pageSize(), this.totalCount()),
  );

  /** Window of page numbers around the current page, five entries at most. */
  protected readonly pages = computed<number[]>(() => {
    const total = this.totalPages();
    if (total <= 0) {
      return [];
    }

    const windowSize = 5;
    let end = Math.min(total, Math.max(this.pageNumber(), 1) + 2);
    let start = Math.max(1, end - windowSize + 1);
    end = Math.min(total, start + windowSize - 1);

    const result: number[] = [];
    for (let page = start; page <= end; page++) {
      result.push(page);
    }

    return result;
  });

  protected goToPage(page: number): void {
    if (this.disabled()) {
      return;
    }

    const highest = Math.max(this.totalPages(), 1);
    const target = Math.min(Math.max(page, 1), highest);

    if (target !== this.pageNumber()) {
      this.pageChange.emit(target);
    }
  }

  protected onPageSizeChange(event: Event): void {
    const selected = Number((event.target as HTMLSelectElement).value);

    if (!Number.isNaN(selected) && selected !== this.pageSize()) {
      this.pageSizeChange.emit(selected);
    }
  }
}
