import { ComponentFixture, TestBed } from '@angular/core/testing';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { Pagination } from './pagination';

describe('Pagination', () => {
  let fixture: ComponentFixture<Pagination>;

  function createPagination(
    overrides: {
      pageNumber?: number;
      pageSize?: number;
      totalCount?: number;
      totalPages?: number;
      hasPreviousPage?: boolean;
      hasNextPage?: boolean;
      disabled?: boolean;
    } = {},
  ): void {
    fixture = TestBed.createComponent(Pagination);
    fixture.componentRef.setInput('pageNumber', overrides.pageNumber ?? 1);
    fixture.componentRef.setInput('pageSize', overrides.pageSize ?? 10);
    fixture.componentRef.setInput('totalCount', overrides.totalCount ?? 0);
    fixture.componentRef.setInput('totalPages', overrides.totalPages ?? 0);
    fixture.componentRef.setInput('hasPreviousPage', overrides.hasPreviousPage ?? false);
    fixture.componentRef.setInput('hasNextPage', overrides.hasNextPage ?? false);
    fixture.componentRef.setInput('disabled', overrides.disabled ?? false);
    fixture.detectChanges();
  }

  function pageButtons(): HTMLButtonElement[] {
    const element = fixture.nativeElement as HTMLElement;

    return Array.from(element.querySelectorAll('.pagination__page')) as HTMLButtonElement[];
  }

  function buttonWithLabel(label: string): HTMLButtonElement {
    const element = fixture.nativeElement as HTMLElement;
    const button = (Array.from(element.querySelectorAll('button')) as HTMLButtonElement[]).find(
      (candidate) => candidate.textContent?.trim() === label,
    );

    if (!button) {
      throw new Error(`No button labelled "${label}" was rendered.`);
    }

    return button;
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [Pagination] }).compileComponents();
  });

  it('reports an empty collection without rendering page buttons', () => {
    createPagination({ totalCount: 0, totalPages: 0 });

    expect(fixture.nativeElement.querySelector('.pagination__summary')?.textContent).toContain('No results');
    expect(pageButtons()).toHaveLength(0);
    expect(buttonWithLabel('Previous').disabled).toBe(true);
    expect(buttonWithLabel('Next').disabled).toBe(true);
  });

  it('describes the visible row range', () => {
    createPagination({ pageNumber: 3, pageSize: 10, totalCount: 42, totalPages: 5, hasPreviousPage: true, hasNextPage: true });

    const summary = fixture.nativeElement.querySelector('.pagination__summary')?.textContent ?? '';

    expect(summary).toContain('21');
    expect(summary).toContain('30');
    expect(summary).toContain('42');
  });

  it('renders a five button window around the current page', () => {
    createPagination({ pageNumber: 10, pageSize: 10, totalCount: 200, totalPages: 20, hasPreviousPage: true, hasNextPage: true });

    expect(pageButtons().map((button) => button.textContent?.trim())).toEqual(['8', '9', '10', '11', '12']);
    expect(fixture.nativeElement.querySelector('.pagination__page.is-current')?.textContent?.trim()).toBe('10');
  });

  it('clamps the window at the first and last page', () => {
    createPagination({ pageNumber: 1, pageSize: 10, totalCount: 30, totalPages: 3, hasNextPage: true });
    expect(pageButtons().map((button) => button.textContent?.trim())).toEqual(['1', '2', '3']);

    createPagination({ pageNumber: 3, pageSize: 10, totalCount: 30, totalPages: 3, hasPreviousPage: true });
    expect(pageButtons().map((button) => button.textContent?.trim())).toEqual(['1', '2', '3']);
  });

  it('emits the requested page', () => {
    createPagination({ pageNumber: 2, pageSize: 10, totalCount: 30, totalPages: 3, hasPreviousPage: true, hasNextPage: true });
    const pageChange = vi.fn();
    fixture.componentInstance.pageChange.subscribe(pageChange);

    buttonWithLabel('3').click();

    expect(pageChange).toHaveBeenCalledWith(3);
  });

  it('emits a new page size from the select', () => {
    createPagination({ pageNumber: 1, pageSize: 10, totalCount: 30, totalPages: 3, hasNextPage: true });
    const pageSizeChange = vi.fn();
    fixture.componentInstance.pageSizeChange.subscribe(pageSizeChange);

    const select = (fixture.nativeElement as HTMLElement).querySelector('select') as HTMLSelectElement | null;
    expect(select).not.toBeNull();
    select!.value = '20';
    select!.dispatchEvent(new Event('change'));

    expect(pageSizeChange).toHaveBeenCalledWith(20);
  });

  it('ignores clicks while disabled', () => {
    createPagination({ pageNumber: 2, pageSize: 10, totalCount: 30, totalPages: 3, hasPreviousPage: true, hasNextPage: true, disabled: true });
    const pageChange = vi.fn();
    fixture.componentInstance.pageChange.subscribe(pageChange);

    buttonWithLabel('3').click();

    expect(pageChange).not.toHaveBeenCalled();
    expect(buttonWithLabel('3').disabled).toBe(true);
  });
});
