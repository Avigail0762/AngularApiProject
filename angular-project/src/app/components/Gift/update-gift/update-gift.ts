import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { GiftsService } from '../../../services/gifts-service';
import { GiftDTO } from '../../../models/Dto/giftDto';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-update-gift',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './update-gift.html',
  styleUrl: './update-gift.scss',
})
export class UpdateGift {
    giftsSrv: GiftsService = inject(GiftsService)
    route = inject(ActivatedRoute);
    router = inject(Router);
    giftId!: number;
    gift?: GiftDTO;
  canUpdate: boolean = true;

    giftForm: FormGroup = new FormGroup({
      name: new FormControl('', Validators.required),
      price: new FormControl(0, [Validators.required, Validators.min(10)]),
      description: new FormControl(''),
      donorId: new FormControl(0, Validators.required),
      category: new FormControl('')
    });
      ngOnInit() {
    this.giftId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.giftId) {
      this.giftsSrv.getById(this.giftId).subscribe(g => {
        this.gift = g;
        this.giftForm.patchValue(this.gift);
            // If there are buyers (tickets), prevent updating
            this.canUpdate = (g.buyersNumber ?? 0) === 0;
      });
    }
  }


  updateGift() {
    if (!this.giftId || this.giftForm.invalid || !this.canUpdate) return;

    const updatedGift: GiftDTO = {
      ...this.gift,
      ...this.giftForm.value
    };

    this.giftsSrv.update(updatedGift, this.giftId).subscribe(() => {
      alert('Gift updated successfully!');
      this.router.navigate(['/gifts']); // חזרה לרשימת מתנות
    });
  }
    cancel() {
    this.router.navigate(['/gifts']);
  }
}
