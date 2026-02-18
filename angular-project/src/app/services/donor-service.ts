import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Donor } from '../models/donorModel';
import { DonorDTO } from '../models/Dto/donorDto';

@Injectable({
  providedIn: 'root'
})
export class DonorService {
  httpClient: HttpClient = inject(HttpClient);
  BASE_URL: string = 'https://localhost:7253/api/donor';

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
    return this.httpClient.get<Donor[]>(this.BASE_URL, { headers: this.getAuthHeader() });
  }

  getByName(firstName: string, lastName: string) {
    return this.httpClient.get<Donor>(this.BASE_URL + '/name', { params: { firstName, lastName }, headers: this.getAuthHeader() }
    );
  }
  update(item: DonorDTO, id: number) {
    return this.httpClient.put<DonorDTO>(this.BASE_URL + '/' + id, item, { headers: this.getAuthHeader() });
  }
  add(item: DonorDTO) {
    return this.httpClient.post<DonorDTO>(this.BASE_URL, item, { headers: this.getAuthHeader() });
  }
  delete(id: number) {
    return this.httpClient.delete<void>(this.BASE_URL + `/${id}`, { headers: this.getAuthHeader() });
  }

}
