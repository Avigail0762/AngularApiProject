import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Ticket } from '../models/ticketModel';
@Injectable({
  providedIn: 'root',
})
export class TicketService {

httpClient: HttpClient = inject(HttpClient);

  BASE_URL: string = 'https://localhost:7253/api/purchases';

  getAuthHeader() {
    const token = localStorage.getItem('authToken') || '';
    if (!token) {
      alert('אין טוקן תקף, מעביר להתחברות...');
      // פה בעתיד תעשי ניתוב ל-login
      return;
    }
    const httpHeaders = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return httpHeaders;
  }

  // 🎟 שליפת כל הכרטיסים של מתנה מסוימת (למנהל)
  getTicketsByGiftId(giftId: number) {
    return this.httpClient.get<Ticket[]>(
      `${this.BASE_URL}/tickets-by-gift/${giftId}`,
      { headers: this.getAuthHeader() }
    );
  }

  
}
