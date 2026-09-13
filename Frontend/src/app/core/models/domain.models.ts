export interface StudentProfile { studentId:number; userId:number; indexNumber:string; fullName:string; email:string; phoneNumber?:string|null; address?:string|null; facultyId:number; facultyCode:string; facultyName:string; isActive:boolean; emailVerified:boolean; deactivatedAt?:string|null; }
export interface StudentListItem { studentId:number; indexNumber:string; fullName:string; email:string; facultyId:number; facultyCode:string; isActive:boolean; }
export interface StudentActivitySummary { hostelApplications:number; labBookings:number; eventRegistrations:number; complaints:number; certificateRequests:number; outstandingFees:number; }
export interface StudentMasterCheck { indexNumber:string; exists:boolean; isActive:boolean; alreadyRegistered:boolean; }
export interface StudentMaster { studentMasterId:number; indexNumber:string; fullName:string; officialEmail?:string|null; facultyId:number; facultyCode:string; facultyName:string; intakeYear:number; isActive:boolean; alreadyRegistered:boolean; }
export interface Faculty { facultyId:number; code:string; name:string; isActive:boolean; }
export interface AdminAccount { userId:number; email:string; isActive:boolean; emailVerified:boolean; createdAt:string; updatedAt?:string|null; }

export interface Hostel { hostelId:number; name:string; location?:string|null; isActive:boolean; createdAt:string; }
export interface Room { roomId:number; hostelId:number; roomNumber:string; capacity:number; isActive:boolean; createdAt:string; }
export interface HostelApplication { hostelApplicationId:number; studentId:number; preferredHostelId:number; preferredHostelName?:string|null; assignedRoomId?:number|null; assignedRoomNumber?:string|null; reviewedByUserId?:number|null; status:string; academicYear:string; semester:string; requestedAt:string; reviewedAt?:string|null; }
export interface HostelAvailability { hostelId:number; academicYear:string; semester:string; totalCapacity:number; occupied:number; available:number; }

export interface Lab { labId:number; code:string; name:string; labType:string; capacity:number; isActive:boolean; createdAt:string; }
export interface LabSlot { labTimeSlotId:number; labId:number; dayOfWeek:string; startTime:string; endTime:string; isActive:boolean; }
export interface LabSeat { labSeatId:number; labId:number; seatNumber:string; isActive:boolean; }
export interface LabBooking { labBookingId:number; studentId:number; labId:number; labName?:string|null; labTimeSlotId:number; dayOfWeek?:string|null; startTime:string; endTime:string; labSeatId?:number|null; seatNumber?:string|null; bookingDate:string; status:string; expiresAt?:string|null; createdAt:string; }

export interface Venue { venueId:number; name:string; venueType:number; location:string; capacity:number; isActive:boolean; createdAt:string; updatedAt?:string|null; }
export interface EventItem { eventId:number; venueId:number; venueName:string; title:string; description?:string|null; startAt:string; endAt:string; capacity:number; registeredCount:number; availableSeats:number; usesReservedSeating:boolean; isPublished:boolean; isActive:boolean; createdAt:string; updatedAt?:string|null; }
export interface EventSeat { eventSeatId:number; eventId:number; seatNumber:string; sectionName?:string|null; rowLabel?:string|null; isActive:boolean; createdAt?:string; updatedAt?:string|null; isAvailable?:boolean; }
export interface EventRegistration { eventRegistrationId:number; eventId?:number; eventTitle:string; studentId?:number; studentName?:string; eventSeatId?:number|null; seatNumber?:string|null; status:number; expiresAt?:string|null; registeredAt?:string; startAt?:string; venueName?:string; }

export interface ComplaintCategory { complaintCategoryId:number; name:string; description?:string|null; isActive:boolean; }
export interface Complaint { complaintId:number; studentId:number; complaintCategoryId:number; categoryName:string; description:string; status:number; resolutionNote?:string|null; resolvedAt?:string|null; statusChangedByUserId?:number|null; }

export interface CertificateType { certificateTypeId:number; name:string; description?:string|null; isActive:boolean; createdAt:string; }
export interface CertificateRequest { certificateRequestId:number; certificateTypeId:number; certificateTypeName:string; studentId:number; studentName:string; reviewedByUserId?:number|null; reason?:string|null; status:string; reviewNote?:string|null; requestedAt:string; reviewedAt?:string|null; }

export interface FeeType { feeTypeId:number; name:string; description?:string|null; isActive:boolean; createdAt?:string; updatedAt?:string|null; }
export interface FeePayment { feePaymentId:number; studentId:number; feeTypeId:number; feeTypeName:string; amount:number; billingPeriod:string; status:string; dueDate:string; receiptNumber?:string|null; paidAt?:string|null; paymentMethod?:string|null; paymentReference?:string|null; }
export interface FeeReceipt { feePaymentId:number; studentId:number; feeType:string; billingPeriod:string; amount:number; receiptNumber:string; paidAt:string; paymentMethod:string; paymentReference:string; }

export interface NotificationItem { notificationId:number; studentId:number; type:string; title:string; message:string; isRead:boolean; createdAt:string; readAt?:string|null; }
export interface SystemSetting { systemSettingId:number; settingKey:string; settingValue:string; description?:string|null; updatedByUserId?:number|null; updatedAt?:string|null; }

export interface StudentDashboard { currentHostelApplication?: { hostelApplicationId:number; status:string; hostelName:string; roomNumber?:string|null } | null; upcomingLabBookings:{labBookingId:number;labId:number;labName:string;bookingDate:string;startTime:string;endTime:string}[]; registeredEvents:{eventRegistrationId:number;eventId:number;title:string;startAt:string;venue:string}[]; certificateRequests:{certificateRequestId:number;certificateType:string;status:string;requestedAt:string}[]; unreadNotifications:number; }
export interface AdminDashboard { totalRegisteredStudents:number; pendingHostelApplications:number; openComplaints:{category:string;status:string;count:number}[]; upcomingEvents:{eventId:number;title:string;startAt:string;capacity:number;registrationCount:number}[]; pendingCertificateRequests:number; feeCollection:{paidAmount:number;outstandingAmount:number;paidCount:number;outstandingCount:number}; }
