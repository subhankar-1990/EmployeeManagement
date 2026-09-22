import { Routes } from '@angular/router';

/**
 * Routes of the employees feature, lazily loaded from `app.routes.ts`.
 *
 * `new` and `:id/edit` are declared before `:id` so the literal segments always win.
 */
export const EMPLOYEES_ROUTES: Routes = [
  {
    path: '',
    title: 'Employees',
    loadComponent: () => import('./employee-list/employee-list').then((module) => module.EmployeeList),
  },
  {
    path: 'new',
    title: 'Add employee',
    loadComponent: () => import('./employee-form/employee-form').then((module) => module.EmployeeForm),
  },
  {
    path: ':id/edit',
    title: 'Edit employee',
    loadComponent: () => import('./employee-form/employee-form').then((module) => module.EmployeeForm),
  },
  {
    path: ':id',
    title: 'Employee details',
    loadComponent: () => import('./employee-detail/employee-detail').then((module) => module.EmployeeDetail),
  },
];
