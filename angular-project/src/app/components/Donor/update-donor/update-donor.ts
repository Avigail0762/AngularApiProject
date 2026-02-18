import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DonorService } from '../../../services/donor-service';
import { DonorDTO } from '../../../models/Dto/donorDto';
import { Donor } from '../../../models/donorModel';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-update-donor',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './update-donor.html',
  styleUrl: './update-donor.scss',
})
export class UpdateDonor {

  donorSrv = inject(DonorService);
  route = inject(ActivatedRoute);
  router = inject(Router);

  donorId!: number;   

  donorForm: FormGroup = new FormGroup({
    firstName: new FormControl('', Validators.required),
    lastName: new FormControl('', Validators.required),
    phoneNumber: new FormControl('', Validators.minLength(10)),
    email: new FormControl('', [Validators.required, Validators.email]),
    address: new FormControl('')
  });

  ngOnInit() {
    const firstName = this.route.snapshot.paramMap.get('firstName')!;
    const lastName = this.route.snapshot.paramMap.get('lastName')!;

    this.donorSrv.getByName(firstName, lastName).subscribe((d: Donor) => {
      this.donorId = d.id;              
      this.donorForm.patchValue(d);     
    });
  }

  updateDonor() {
    if (this.donorForm.invalid) return;

    const updatedDonor: DonorDTO = this.donorForm.value;

    this.donorSrv.update(updatedDonor, this.donorId).subscribe(() => {
      alert('Donor updated!');
      this.router.navigate(['/donors']);
    });
  }
    cancel() {
    this.router.navigate(['/donors']);
  }
}
