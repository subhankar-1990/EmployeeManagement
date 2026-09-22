/**
 * UI level view of the contract generated in `api-client.ts`.
 *
 * The NSwag generated classes are re-exported so feature code has a single import to reach for,
 * and the constants below mirror the rules enforced by the API
 * (`EmployeeManagement.Application.Services.Employees.Validations`).
 */
export {
  ApiException,
  CreateEmployeeRequest,
  EmployeeResponse,
  EmployeesClient,
  PagedResponseOfEmployeeResponse,
  ProblemDetails,
  UpdateEmployeeRequest,
} from '../services/api-client';

/** Field length ceilings enforced by the API validators. */
export const EMPLOYEE_LIMITS = {
  name: 100,
  mobile: 20,
  email: 150,
} as const;

/** Page sizes offered by the list screen. Server accepts 1-100. */
export const PAGE_SIZE_OPTIONS = [5, 10, 20, 50] as const;

/** Page size used until the user picks another one. */
export const DEFAULT_PAGE_SIZE = 10;

/** One based first page. */
export const DEFAULT_PAGE_NUMBER = 1;

/** Default debounce applied to the search box. */
export const SEARCH_DEBOUNCE_MS = 300;

/** Arguments accepted by `GET /api/v1/employees`. */
export interface EmployeeListQuery {
  pageNumber: number;
  pageSize: number;
  /** Optional case insensitive term matched against name, mobile and email. */
  search?: string;
}

/** Shape of the employee form, shared by the create and edit screens. */
export interface EmployeeFormValue {
  name: string;
  mobile: string;
  email: string;
  isActive: boolean;
}
