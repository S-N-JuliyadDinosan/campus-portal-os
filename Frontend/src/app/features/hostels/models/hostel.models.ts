export interface Hostel { hostelId:number; name:string; location?:string|null; isActive:boolean; createdAt:string; }
export interface Room { roomId:number; hostelId:number; roomNumber:string; capacity:number; isActive:boolean; createdAt:string; }
export interface HostelApplication { hostelApplicationId:number; studentId:number; studentName?:string; studentIndexNumber?:string; preferredHostelId:number; preferredHostelName?:string|null; assignedRoomId?:number|null; assignedRoomNumber?:string|null; reviewedByUserId?:number|null; status:string; academicYear:string; semester:string; requestedAt:string; reviewedAt?:string|null; }
export interface HostelAvailability { hostelId:number; academicYear:string; semester:string; totalCapacity:number; occupied:number; available:number; }
