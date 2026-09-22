import { Injectable, inject } from '@angular/core';
import { Observable, catchError, from, throwError } from 'rxjs';
import {
  CreateEmployeeRequest,
  EmployeeResponse,
  EmployeesClient,
  PagedResponseOfEmployeeResponse,
  UpdateEmployeeRequest,
} from '../models/employee.model';
import type { EmployeeListQuery } from '../models/employee.model';
import { toApiError } from '../utils/api-error';
import { createIdempotencyKey } from '../utils/idempotency-key';

/**
 * Thin, observable flavoured facade over the generated `EmployeesClient`.
 *
 * Responsibilities:
 * - expose the employee endpoints as RxJS observables so they compose with the router, forms and
 *   operators such as `switchMap` used by the list screen;
 * - translate every failure into an {@link ApiError} carrying a displayable message plus the field
 *   level validation errors returned by the API.
 */
@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private readonly client = inject(EmployeesClient);

  /** One page of employees ordered by employee id. */
  getEmployees(query: EmployeeListQuery): Observable<PagedResponseOfEmployeeResponse> {
    const search = query.search?.trim();

    return from(
      this.client.getEmployees(query.pageNumber, query.pageSize, search ? search : undefined),
    ).pipe(catchError(failWithApiError));
  }

  /** A single employee by id. */
  getEmployeeById(id: number): Observable<EmployeeResponse> {
    return from(this.client.getEmployeeById(id)).pipe(catchError(failWithApiError));
  }

  /**
   * Creates an employee.
   *
   * @param request Employee data.
   * @param idempotencyKey Value for the `Idempotency-Key` header. Generated when omitted so a
   * retried submission is answered from the API's idempotency store instead of inserting twice.
   */
  createEmployee(
    request: CreateEmployeeRequest,
    idempotencyKey: string = createIdempotencyKey(),
  ): Observable<EmployeeResponse> {
    return from(this.client.createEmployee(idempotencyKey, request)).pipe(catchError(failWithApiError));
  }

  /** Replaces the mutable fields of an existing employee. */
  updateEmployee(id: number, request: UpdateEmployeeRequest): Observable<void> {
    return from(this.client.updateEmployee(id, request)).pipe(catchError(failWithApiError));
  }

  /** Deletes an employee by id. */
  deleteEmployee(id: number): Observable<void> {
    return from(this.client.deleteEmployee(id)).pipe(catchError(failWithApiError));
  }
}

function failWithApiError(error: unknown): Observable<never> {
  return throwError(() => toApiError(error));
}
