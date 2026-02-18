import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { DonorService } from '../../../services/donor-service';
import { GiftsService } from '../../../services/gifts-service';
import { Donor } from '../../../models/donorModel';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-donors',
  imports: [FormsModule, CommonModule, RouterOutlet],
  templateUrl: './donors.html',
  styleUrl: './donors.scss'
})
export class Donors {
  donorSrv: DonorService = inject(DonorService)
  giftsSrv: GiftsService = inject(GiftsService)
  router = inject(Router)
  donors$ = this.donorSrv.getAll();
  donor = new Donor();  
  donorIdsWithGifts: Set<number> = new Set<number>();

  firstNameFilter: string = '';
lastNameFilter: string = '';

    ngOnInit() {
      this.refreshDonorGiftSet();
    }

    getAll(){
      this.donors$ = this.donorSrv.getAll();
      this.refreshDonorGiftSet();
    }
    // geByName(firstName: string, lastName: string){
    //   this.donorSrv.geByName(firstName, lastName).subscribe(data =>{
    //     this.donor = data;
    //   })
    // } 
    delete(id: number){
      // Frontend guard: prevent deleting donors who have gifts
      if (this.donorIdsWithGifts.has(id)) {
        alert('לא ניתן למחוק תורם שיש לו מתנות משויכות');
        return;
      }
      this.donorSrv.delete(id).subscribe({
        next: () => {
          this.getAll();
        },
        error: (err) => {
          const msg = err?.error?.message || 'מחיקה נכשלה. ייתכן ולתורם קיימות מתנות.';
          alert(msg);
        }
      })
    }
    addDonor(){
    this.router.navigate(['/donors/add']);
  }

    updateDonor(firstName: string, lastName: string){
  this.router.navigate(['/donors/update', firstName, lastName]);
  }

  private refreshDonorGiftSet() {
    this.giftsSrv.getAll().subscribe(gifts => {
      const ids = new Set<number>();
      for (const g of gifts ?? []) {
        if (g?.donorId) ids.add(g.donorId);
      }
      this.donorIdsWithGifts = ids;
    })
  }
searchByName() {
  if (!this.firstNameFilter || !this.lastNameFilter) {
    this.getAll();
    return;
  }

  this.donors$ = this.donorSrv
    .getByName(this.firstNameFilter, this.lastNameFilter)
    .pipe(map(d => d ? [d] : []));
}


clearFilter() {
  this.firstNameFilter = '';
  this.lastNameFilter = '';
  this.getAll();
}

}
