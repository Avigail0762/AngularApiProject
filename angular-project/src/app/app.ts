import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { RouterOutlet, RouterLinkWithHref, Router } from '@angular/router';
import { Gifts } from './components/Gift/gifts/gifts';
import { Donors } from './components/Donor/donors/donors';
import { Lottery } from './components/lottery/lottery';
import { Auth } from './components/auth/auth';
import { AddGift } from './components/Gift/add-gift/add-gift';
import { AddDonor } from './components/Donor/add-donor/add-donor';
import { UpdateGift } from './components/Gift/update-gift/update-gift';
import { AuthService } from './services/auth-service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Gifts, Donors, Lottery, Auth, AddGift, AddDonor, UpdateGift, RouterLinkWithHref],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit, OnDestroy {

  Role: 'manager' | 'user' | 'userWithoutToken' | null = null;

  router = inject(Router);

  private onAuthTokenUpdated = () => this.updateRoleFromService(); // חידוש התפקיד כאשר הטוקן משתנה

  constructor(private auth: AuthService) {
    this.updateRoleFromService(); // אתחול התפקיד בהקמת הקומפוננטה
  }

  ngOnInit() {
    // אם מישהו ישלח עדכון בשם זה, אז תקרא לפונ' הנ"ל
    window.addEventListener('authTokenUpdated', this.onAuthTokenUpdated);
  }

  ngOnDestroy() {
    window.removeEventListener('authTokenUpdated', this.onAuthTokenUpdated);
  }

  private updateRoleFromService() {
    this.Role = this.auth.getStoredRole();
  }

  logOut(){
    this.Role = 'userWithoutToken';
    this.auth.logout();
    this.router.navigate(['/home']);
  }
}