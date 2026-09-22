import {
  __spreadValues
} from "./chunk-GOMI4DH3.js";

// src/app/features/employees/employees.routes.ts
var EMPLOYEES_ROUTES = [
  __spreadValues({
    path: "",
    title: "Employees",
    loadComponent: () => import("./chunk-UCJWJEHN.js").then((module) => module.EmployeeList)
  }, false ? { \u0275entryName: "src/app/features/employees/employee-list/employee-list.ts" } : {}),
  __spreadValues({
    path: "new",
    title: "Add employee",
    loadComponent: () => import("./chunk-2NUO3JRT.js").then((module) => module.EmployeeForm)
  }, false ? { \u0275entryName: "src/app/features/employees/employee-form/employee-form.ts" } : {}),
  __spreadValues({
    path: ":id/edit",
    title: "Edit employee",
    loadComponent: () => import("./chunk-2NUO3JRT.js").then((module) => module.EmployeeForm)
  }, false ? { \u0275entryName: "src/app/features/employees/employee-form/employee-form.ts" } : {}),
  __spreadValues({
    path: ":id",
    title: "Employee details",
    loadComponent: () => import("./chunk-NKLZKAD7.js").then((module) => module.EmployeeDetail)
  }, false ? { \u0275entryName: "src/app/features/employees/employee-detail/employee-detail.ts" } : {})
];
export {
  EMPLOYEES_ROUTES
};
//# debugId=ca1f9fee-05ea-5a39-9672-5cfc33bc4ff6
//# sourceMappingURL=chunk-MWYQ2XMG.js.map
