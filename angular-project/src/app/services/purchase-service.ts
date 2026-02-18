import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Ticket } from '../models/ticketModel';
import { Gift } from '../models/giftModel';
import { UserDTO } from '../models/Dto/userDto';

@Injectable({
  providedIn: 'root',
})
export class PurchaseService {

  httpClient: HttpClient = inject(HttpClient);
  BASE_URL: string = 'https://localhost:7253/api/purchases';

  getAuthHeader() {
    const token = localStorage.getItem('authToken') || '';
    if (!token) {
      alert('אין טוקן תקף, מעביר להתחברות...');
      //להעביר להתחברות
      return;
    }
    const httpHeaders = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return httpHeaders;
  }

  getTicketsByGift(giftId: number) {
    return this.httpClient.get<Ticket[]>(`${this.BASE_URL}/tickets-by-gift/${giftId}`, { headers: this.getAuthHeader() });
  }

  getGiftsByPrice() {
    return this.httpClient.get<Gift[]>(`${this.BASE_URL}/gifts-by-price`, { headers: this.getAuthHeader() });
  }

  getGiftsByBuyers() {
    return this.httpClient.get<Gift[]>(`${this.BASE_URL}/gifts-by-buyers`, { headers: this.getAuthHeader() });
  }

  getBuyersByGift(giftId: number) {
    return this.httpClient.get<UserDTO[]>(`${this.BASE_URL}/buyers-by-gift/${giftId}`, { headers: this.getAuthHeader() });
  }
  

}
