import './polyfills.server.mjs';
import {
  __spreadValues
} from "./chunk-LEH5XX4U.mjs";

// src/app/features/employees/employees.routes.ts
var EMPLOYEES_ROUTES = [
  __spreadValues({
    path: "",
    title: "Employees",
    loadComponent: () => import("./chunk-7NEHYDX5.mjs").then((module) => module.EmployeeList)
  }, true ? { \u0275entryName: "src/app/features/employees/employee-list/employee-list.ts" } : {}),
  __spreadValues({
    path: "new",
    title: "Add employee",
    loadComponent: () => import("./chunk-JHNLYAOT.mjs").then((module) => module.EmployeeForm)
  }, true ? { \u0275entryName: "src/app/features/employees/employee-form/employee-form.ts" } : {}),
  __spreadValues({
    path: ":id/edit",
    title: "Edit employee",
    loadComponent: () => import("./chunk-JHNLYAOT.mjs").then((module) => module.EmployeeForm)
  }, true ? { \u0275entryName: "src/app/features/employees/employee-form/employee-form.ts" } : {}),
  __spreadValues({
    path: ":id",
    title: "Employee details",
    loadComponent: () => import("./chunk-ONA4IWGM.mjs").then((module) => module.EmployeeDetail)
  }, true ? { \u0275entryName: "src/app/features/employees/employee-detail/employee-detail.ts" } : {})
];
export {
  EMPLOYEES_ROUTES
};
//# sourceMappingURL=chunk-VP6RE5W4.mjs.map
