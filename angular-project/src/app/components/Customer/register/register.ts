import { Component, inject } from '@angular/core';
import { UserDTO } from '../../../models/Dto/userDto';
import { CustomerService } from '../../../services/customer-service';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {

  customerService = inject(CustomerService);
  router = inject(Router);

  registerForm = new FormGroup({
    username: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(3),
      Validators.maxLength(30),
      Validators.pattern(/^[A-Za-z0-9_]+$/)
    ]),
    phone: new FormControl<string>('', [
      Validators.pattern(/^\d{7,15}$/)
    ]),
    email: new FormControl<string>('', [
      Validators.required,
      Validators.email
    ]),
    password: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(6)
    ])
  });

  loading = false;

  register() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }
    this.loading = true;
    const dto = this.registerForm.value as UserDTO;
    this.customerService.registerCustomer(dto).subscribe({
      next: () => {
        alert('ההרשמה הצליחה! עוברים למסך התחברות...');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        alert('שגיאה בהרשמה: ' + (err?.error?.message || err.message || 'אירעה תקלה'));
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}
