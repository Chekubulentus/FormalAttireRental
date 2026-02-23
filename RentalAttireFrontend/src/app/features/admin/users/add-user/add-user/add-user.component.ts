import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmployeeDTO } from '../../../../../data/models/DTOs/Employees/employee-dto';

@Component({
  selector: 'app-add-user',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './add-user.component.html',
  styleUrl: './add-user.component.scss',
})
export class AddUserComponent {
  @Output() closeForm = new EventEmitter<void>();
  @Input() newEmployee = new EmployeeDTO();


  closed() {
    this.closeForm.emit();
  }
}
