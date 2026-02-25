import { CreateEmployeeCommand } from "../../../../data/models/DTOs/Employees/create-employee";
import { Injectable } from "@angular/core";


@Injectable({ providedIn: 'root' })
export class CreateEmployeeValidator {
    validate(command: CreateEmployeeCommand): string[] {
        const errors: string[] = [];
        if(!command.employeeCode || command.employeeCode.trim() === '')
            errors.push('Employee code is required.');

        if(!command.department || command.department.trim() === '')
            errors.push('Department is required.');

        if(!command.rolePosition || command.rolePosition.trim() === '')
            errors.push('Role position is required.');

        if(!command.email || command.email.trim() === '')
            errors.push('Email is required.');

        if(!command.password || command.password.trim() === '')
            errors.push('Password is required.');

        if(!command.person || !command.person == null)
            errors.push('Person information is required.');

        return errors;
    }

    private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }
}