export interface Lab { labId:number; code:string; name:string; labType:string; capacity:number; isActive:boolean; createdAt:string; }
export interface LabSlot { labTimeSlotId:number; labId:number; dayOfWeek:string; startTime:string; endTime:string; isActive:boolean; }
export interface LabSeat { labSeatId:number; labId:number; seatNumber:string; isActive:boolean; }
export interface LabBooking { labBookingId:number; studentId:number; studentName?:string; studentIndexNumber?:string; labId:number; labName?:string|null; labTimeSlotId:number; dayOfWeek?:string|null; startTime:string; endTime:string; labSeatId?:number|null; seatNumber?:string|null; bookingDate:string; status:string; expiresAt?:string|null; createdAt:string; }
