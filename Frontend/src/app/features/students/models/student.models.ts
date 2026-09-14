export interface StudentProfile { studentId:number; userId:number; indexNumber:string; fullName:string; email:string; phoneNumber?:string|null; address?:string|null; facultyId:number; facultyCode:string; facultyName:string; isActive:boolean; emailVerified:boolean; deactivatedAt?:string|null; }
export interface StudentListItem { studentId:number; indexNumber:string; fullName:string; email:string; facultyId:number; facultyCode:string; isActive:boolean; }
export interface StudentActivitySummary { hostelApplications:number; labBookings:number; eventRegistrations:number; complaints:number; certificateRequests:number; outstandingFees:number; }
export interface StudentMasterCheck { indexNumber:string; exists:boolean; isActive:boolean; alreadyRegistered:boolean; }
export interface StudentMaster { studentMasterId:number; indexNumber:string; fullName:string; officialEmail?:string|null; facultyId:number; facultyCode:string; facultyName:string; intakeYear:number; isActive:boolean; alreadyRegistered:boolean; }
export interface Faculty { facultyId:number; code:string; name:string; isActive:boolean; }
export interface AdminAccount { userId:number; email:string; isActive:boolean; emailVerified:boolean; createdAt:string; updatedAt?:string|null; }
