import { Donor } from "./donorModel";

export class Gift{
    id!: number
    name!: string;
    //change to Donor type
    donor?: Donor;
    price!: number;
    description?: string;
    donorId!: number;
    buyersNumber: number = 0;
    category?: string;
    winnerTicketId?: number; 
    isDrawn: boolean = false;
}