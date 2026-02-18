import { Routes } from '@angular/router';
import { Gifts } from './components/Gift/gifts/gifts';
import { AddGift } from './components/Gift/add-gift/add-gift';
import { Donors } from './components/Donor/donors/donors';
import { AddDonor } from './components/Donor/add-donor/add-donor';
import { UpdateGift } from './components/Gift/update-gift/update-gift';
import { Auth } from './components/auth/auth';
import { UpdateDonor } from './components/Donor/update-donor/update-donor';
import { Register } from './components/Customer/register/register';
import { ShoppingCart } from './components/Customer/shopping-cart/shopping-cart';
import { Home } from './components/home/home';
import { Lottery } from './components/lottery/lottery';
import { GiftTicket } from './components/Gift/gift-ticket/gift-ticket';

export const routes: Routes = [
    {path: 'gifts', component: Gifts},
    { path: 'gifts/add', component: AddGift },
    { path: 'gifts/update/:id', component: UpdateGift },
    {path: 'gift-tickets/:id',component: GiftTicket},

    { path: 'donors', component: Donors },
    { path: 'donors/add', component: AddDonor },
    { path: 'donors/update/:firstName/:lastName', component: UpdateDonor },
    
    {path: 'login', component: Auth },
    {path: 'register', component: Register },
    {path: 'cart', component: ShoppingCart },
    {path: 'home', component: Home },
    {path: 'lottery', component: Lottery}

];
