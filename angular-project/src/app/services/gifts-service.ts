import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Gift } from '../models/giftModel';
import { GiftDTO } from '../models/Dto/giftDto';

@Injectable({
  providedIn: 'root'
})

export class GiftsService {
  
  httpClient: HttpClient = inject(HttpClient);
  BASE_URL: string = 'https://localhost:7253/api/gift';

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

  getAll() {
    return this.httpClient.get<Gift[]>(this.BASE_URL, { headers: this.getAuthHeader() });
  }
  getById(id: number) {
    return this.httpClient.get<Gift>(this.BASE_URL + '/' + id, { headers: this.getAuthHeader() });
  }
  update(item: GiftDTO, id: number) {
    return this.httpClient.put<GiftDTO>(this.BASE_URL + '/' + id, item, { headers: this.getAuthHeader() });
  }
  add(item: GiftDTO) {
    return this.httpClient.post<GiftDTO>(this.BASE_URL, item, { headers: this.getAuthHeader() });
  }
  delete(id: number) {
    return this.httpClient.delete<void>(this.BASE_URL + `/${id}`, { headers: this.getAuthHeader() });
  }

}