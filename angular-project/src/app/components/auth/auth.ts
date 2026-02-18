import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/auth-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth',
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.html',
  styleUrl: './auth.scss',
})

export class Auth {
  authSrv: AuthService = inject(AuthService)
  router = inject(Router);
  password = '';
  username = '';


login(username: string, password: string) {
  this.authSrv.login(username, password).subscribe((ok) => {
    if (ok) {
      alert('Login successful!');
         this.router.navigate(['/gifts']);
    } else {
      alert('Login failed. Navigating you to register.');
      this.router.navigate(['/register']);
    }
  });
}
  
}
