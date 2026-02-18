import { Component, inject } from '@angular/core';
import { GiftsService } from '../../../services/gifts-service';
import { Gift } from '../../../models/giftModel';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GiftDTO } from '../../../models/Dto/giftDto';
import { AuthService } from '../../../services/auth-service';
import { Observable } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerService } from '../../../services/customer-service';
import { PurchaseService } from '../../../services/purchase-service';

@Component({
  selector: 'app-gifts',
  imports: [FormsModule, CommonModule],
  templateUrl: './gifts.html',
  styleUrl: './gifts.scss'
})
export class Gifts {

  giftsSrv: GiftsService = inject(GiftsService)
  auth = inject(AuthService);
  customerSrv = inject(CustomerService);
  purchaseSrv = inject(PurchaseService);

  gifts$ = this.giftsSrv.getAll();
  gift$?: Observable<GiftDTO>;

  router = inject(Router);
  activateRoute = inject(ActivatedRoute);

  // מאפיין נוח לתבנית: האם המשתמש מנהל
  get isManager(){
    return this.auth.isManager();
  }

  addGift(){
    // Navigate to the child route under /gifts
    this.router.navigate(['/gifts/add']);
  }

  updateGift(giftId: number){
  this.router.navigate(['/gifts/update', giftId]);
  }

  addToCart(giftId: number){
    const uid = this.auth.getStoredUserId();
    if (uid == null) {
      alert('לא נמצא משתמש מחובר');
      return;
    }
    this.customerSrv.addToCartForCurrentUser(giftId).subscribe();
    alert(`Gift with ID ${giftId} added to cart!`);
  }

  getAll(){
    this.gifts$ = this.giftsSrv.getAll();
  }
  getById(id: number){
  this.gift$ = this.giftsSrv.getById(id);
  }

  delete(id: number){
    this.giftsSrv.delete(id).subscribe(data =>{
      this.gifts$ = this.giftsSrv.getAll();
    })
  }

  getGiftsByPrice(){
    this.gifts$ = this.purchaseSrv.getGiftsByPrice();
  }

  viewTickets = (giftId: number) => {
  this.router.navigate(['/gift-tickets', giftId]);
}
}