import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AuthService } from './auth-service';
import { UserDTO } from '../models/Dto/userDto';
import { Gift } from '../models/giftModel';
import { EMPTY, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

export class CustomerService {
  httpClient: HttpClient = inject(HttpClient);
  BASE_URL: string = 'https://localhost:7253/api/customer';
  auth = inject(AuthService);

  registerCustomer(customerData: UserDTO) {
    return this.httpClient.post<any>(this.BASE_URL + '/register', customerData);
  }

  addToCart(customerId: number, giftId: number) {
    const params = new HttpParams()
      .set('userId', customerId)
      .set('giftId', giftId);
    return this.httpClient.post<void>(
      this.BASE_URL + `/cart/add`,
      null,
      { params, headers: this.getAuthHeader() }
    );
  }

  // עטיפה נוחה: משתמש נוכחי מה-Auth
  addToCartForCurrentUser(giftId: number) {
    const userId = this.auth.getStoredUserId();
    if (userId == null) {
      alert('לא נמצא משתמש מחובר');
      return EMPTY;
    }
    return this.addToCart(userId, giftId);
  }

  removeFromCart(customerId: number, giftId: number) {
    const params = new HttpParams()
      .set('userId', customerId)
      .set('giftId', giftId);
    return this.httpClient.delete<void>(
      this.BASE_URL + `/cart/remove`,
      { params, headers: this.getAuthHeader() }
    );
  }

  removeFromCartForCurrentUser(giftId: number) {
    const userId = this.auth.getStoredUserId();
    if (userId == null) {
      alert('לא נמצא משתמש מחובר');
      return EMPTY;
    }
    return this.removeFromCart(userId, giftId);
  }

  purchase() {
    const userId = this.auth.getStoredUserId();
    if (userId == null) {
      alert('לא נמצא משתמש מחובר');
      return EMPTY;
    }
    return this.httpClient.post<void>(
      this.BASE_URL + `/purchase?userId=${userId}`,
      null,
      { headers: this.getAuthHeader() }
    );
  }

  getCart(customerId: number) {
    return this.httpClient.get<Gift[]>(
      this.BASE_URL + `/cart`, customerId ? { params: new HttpParams().set('userId', customerId), headers: this.getAuthHeader() } :
      { headers: this.getAuthHeader() }
    );
  }

  getCartForCurrentUser() {
    const userId = this.auth.getStoredUserId();
    if (userId == null) {
      alert('לא נמצא משתמש מחובר');
      return of([] as Gift[]);
    }
    return this.getCart(userId);
  }

  private getAuthHeader() {
    const token = localStorage.getItem('authToken') || '';
    if (!token) {
      alert('אין טוקן תקף, מעביר להתחברות...');
      return;
    }
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }
}
