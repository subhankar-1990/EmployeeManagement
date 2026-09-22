import './polyfills.server.mjs';
import {
  ApiException,
  DestroyRef,
  EmployeesClient,
  Injectable,
  Observable,
  ProblemDetails,
  assertInInjectionContext,
  catchError,
  from,
  inject,
  setClassMetadata,
  takeUntil,
  throwError,
  ɵɵdefineInjectable
} from "./chunk-HAJVBCAU.mjs";

// node_modules/@angular/core/fesm2022/rxjs-interop.mjs
/**
 * @license Angular v22.1.7
 * (c) 2010-2026 Google LLC. https://angular.dev/
 * License: MIT
 */
function takeUntilDestroyed(destroyRef) {
  if (!destroyRef) {
    ngDevMode && assertInInjectionContext(takeUntilDestroyed);
    destroyRef = inject(DestroyRef);
  }
  const destroyed$ = new Observable((subscriber) => {
    if (destroyRef.destroyed) {
      subscriber.next();
      return;
    }
    const unregisterFn = destroyRef.onDestroy(subscriber.next.bind(subscriber));
    return unregisterFn;
  });
  return (source) => {
    return source.pipe(takeUntil(destroyed$));
  };
}

// src/app/core/models/employee.model.ts
var EMPLOYEE_LIMITS = {
  name: 100,
  mobile: 20,
  email: 150
};
var PAGE_SIZE_OPTIONS = [5, 10, 20, 50];
var DEFAULT_PAGE_SIZE = 10;
var DEFAULT_PAGE_NUMBER = 1;
var SEARCH_DEBOUNCE_MS = 300;

// src/app/core/utils/api-error.ts
var UNEXPECTED_MESSAGE = "Something went wrong while talking to the API. Please try again.";
var NETWORK_MESSAGE = "The API could not be reached. Check that the API is running and that the browser can reach it.";
var ApiError = class extends Error {
  /** HTTP status code, or `null` when the request never reached the API. */
  status;
  /** Field level messages keyed by the server property name, e.g. `Name`. */
  fieldErrors;
  /** Correlation id written by the API, when it supplied one. */
  traceId;
  constructor(message, status = null, fieldErrors = {}, traceId = null) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.fieldErrors = fieldErrors;
    this.traceId = traceId;
  }
  /** True when the API rejected the payload and reported which fields were at fault. */
  get hasFieldErrors() {
    return Object.keys(this.fieldErrors).length > 0;
  }
};
function toApiError(error) {
  if (error instanceof ApiError) {
    return error;
  }
  if (typeof error === "object" && error !== null && ApiException.isApiException(error)) {
    const problem = parseProblemDetails(error.response);
    return new ApiError(
      messageForStatus(error.status, asString(problem?.["detail"]), asString(problem?.["title"]), asString(problem?.["traceId"])),
      error.status,
      readFieldErrors(problem),
      asString(problem?.["traceId"])
    );
  }
  if (error instanceof ProblemDetails) {
    const problem = error;
    const status = typeof problem["status"] === "number" ? problem["status"] : null;
    return new ApiError(
      messageForStatus(status, asString(problem["detail"]), asString(problem["title"]), asString(problem["traceId"])),
      status,
      readFieldErrors(problem),
      asString(problem["traceId"])
    );
  }
  if (error instanceof Error) {
    return new ApiError(error.name === "TypeError" ? NETWORK_MESSAGE : error.message || UNEXPECTED_MESSAGE);
  }
  return new ApiError(UNEXPECTED_MESSAGE);
}
function errorMessageOf(error) {
  return toApiError(error).message;
}
function fieldErrorsOf(error) {
  return toApiError(error).fieldErrors;
}
function parseProblemDetails(body) {
  if (!body) {
    return null;
  }
  try {
    const parsed = JSON.parse(body);
    return typeof parsed === "object" && parsed !== null ? parsed : null;
  } catch {
    return null;
  }
}
function readFieldErrors(problem) {
  const errors = problem?.["errors"];
  if (typeof errors !== "object" || errors === null) {
    return {};
  }
  const result = {};
  for (const [field, messages] of Object.entries(errors)) {
    if (Array.isArray(messages)) {
      const values = messages.filter((message) => typeof message === "string");
      if (values.length > 0) {
        result[field] = values;
      }
    } else if (typeof messages === "string" && messages.length > 0) {
      result[field] = [messages];
    }
  }
  return result;
}
function asString(value) {
  return typeof value === "string" && value.length > 0 ? value : null;
}
function messageForStatus(status, detail, title, traceId) {
  switch (status) {
    case 400:
      return detail ?? title ?? "The request was rejected. Review the values you entered and try again.";
    case 403:
      return detail ?? "You are not permitted to perform this operation.";
    case 404:
      return detail ?? "The employee could not be found. It may have been deleted by someone else.";
    case 409:
      return detail ?? title ?? "The request conflicts with the current state of the data.";
    case 429:
      return "Too many requests were sent to the API. Wait a moment and try again.";
    default:
      break;
  }
  if (status === null) {
    return title ?? UNEXPECTED_MESSAGE;
  }
  if (status >= 500) {
    const message = detail ?? "The API reported an unexpected error. Please try again in a moment.";
    return traceId ? `${message} (trace id: ${traceId})` : message;
  }
  return detail ?? title ?? `The request failed with status ${status}.`;
}

// src/app/core/utils/idempotency-key.ts
function createIdempotencyKey() {
  const cryptoApi = globalThis.crypto;
  if (cryptoApi && typeof cryptoApi.randomUUID === "function") {
    return cryptoApi.randomUUID();
  }
  const random = Math.random().toString(36).slice(2, 12);
  return `employee-${Date.now().toString(36)}-${random}`;
}

// src/app/core/services/employee.service.ts
var EmployeeService = class _EmployeeService {
  client = inject(EmployeesClient);
  /** One page of employees ordered by employee id. */
  getEmployees(query) {
    const search = query.search?.trim();
    return from(this.client.getEmployees(query.pageNumber, query.pageSize, search ? search : void 0)).pipe(catchError(failWithApiError));
  }
  /** A single employee by id. */
  getEmployeeById(id) {
    return from(this.client.getEmployeeById(id)).pipe(catchError(failWithApiError));
  }
  /**
   * Creates an employee.
   *
   * @param request Employee data.
   * @param idempotencyKey Value for the `Idempotency-Key` header. Generated when omitted so a
   * retried submission is answered from the API's idempotency store instead of inserting twice.
   */
  createEmployee(request, idempotencyKey = createIdempotencyKey()) {
    return from(this.client.createEmployee(idempotencyKey, request)).pipe(catchError(failWithApiError));
  }
  /** Replaces the mutable fields of an existing employee. */
  updateEmployee(id, request) {
    return from(this.client.updateEmployee(id, request)).pipe(catchError(failWithApiError));
  }
  /** Deletes an employee by id. */
  deleteEmployee(id) {
    return from(this.client.deleteEmployee(id)).pipe(catchError(failWithApiError));
  }
  static \u0275fac = function EmployeeService_Factory(__ngFactoryType__) {
    return new (__ngFactoryType__ || _EmployeeService)();
  };
  static \u0275prov = /* @__PURE__ */ \u0275\u0275defineInjectable({ token: _EmployeeService, factory: _EmployeeService.\u0275fac, providedIn: "root" });
};
(() => {
  (typeof ngDevMode === "undefined" || ngDevMode) && setClassMetadata(EmployeeService, [{
    type: Injectable,
    args: [{ providedIn: "root" }]
  }], null, null);
})();
function failWithApiError(error) {
  return throwError(() => toApiError(error));
}

export {
  takeUntilDestroyed,
  EMPLOYEE_LIMITS,
  PAGE_SIZE_OPTIONS,
  DEFAULT_PAGE_SIZE,
  DEFAULT_PAGE_NUMBER,
  SEARCH_DEBOUNCE_MS,
  errorMessageOf,
  fieldErrorsOf,
  EmployeeService
};
//# sourceMappingURL=chunk-WPQ6TRVZ.mjs.map
