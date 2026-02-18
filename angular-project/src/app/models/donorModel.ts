import { Gift } from './giftModel';

export class Donor{
    id!: number;
    firstName!: string;
    lastName!: string;
    phoneNumber?: string;
    email!: string;
    address?: string;
    gifts?: Gift[];
}