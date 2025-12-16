# Orders Marketplace Flow - Implementation Summary

**Date**: 2024-12-16  
**Status**: ✅ Completed  
**Mode**: Marketplace (Grab Order) System

---

## 🎯 Business Flow

### Complete Order Lifecycle

```
┌──────────────────────────────────────────────────────────────┐
│  1. Agent Creates Order (Marketplace)                        │
│     - Select Listing                                         │
│     - Set shooting date/time                                 │
│     - Add shooting tasks                                     │
│     - Publish to marketplace                                 │
│     Status: Placed (待抢单)                                  │
│     🔓 Agent can cancel                                      │
└──────────────────────┬───────────────────────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────────────────────┐
│  2. Studio Grabs Order from Marketplace                      │
│     Page: /dashboard/orders/marketplace                      │
│     - Browse all Placed orders                               │
│     - View order details (address, tasks, time)              │
│     - Click "Accept Order" (first come, first served)        │
│     Status: Placed → Accepted                                │
│     🔓 Agent can still cancel                                │
└──────────────────────┬───────────────────────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────────────────────┐
│  3. Studio Assigns Staff                                     │
│     - Assign photographer/videographer                       │
│     - Confirm equipment and schedule                         │
│     - Click "Assign Staff & Schedule"                        │
│     Status: Accepted → Scheduled                             │
│     🔓 Agent can still cancel                                │
└──────────────────────┬───────────────────────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────────────────────┐
│  4. Studio Starts Shooting (Order Locked)                    │
│     - Arrive at shooting location                            │
│     - Click "Start Shooting"                                 │
│     Status: Scheduled → InProgress                           │
│     🔒 Agent CANNOT cancel anymore                           │
└──────────────────────┬───────────────────────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────────────────────┐
│  5. Studio Submits Delivery                                  │
│     - Finish shooting                                        │
│     - Upload photos/videos                                   │
│     - Create Delivery Package                                │
│     - Publish Delivery                                       │
│     Status: InProgress → AwaitingConfirmation                │
│     🔒 Agent CANNOT cancel (but can reject delivery)         │
└──────────────────────┬───────────────────────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────────────────────┐
│  6. Agent Confirms Delivery                                  │
│     - View Delivery Package                                  │
│     - Review photos/videos                                   │
│     - Click "Confirm Delivery"                               │
│     Status: AwaitingConfirmation → Completed                 │
│     ✅ Order automatically completed                         │
└──────────────────────────────────────────────────────────────┘
```

---

## 🔄 Order Status Flow

```
Placed (待抢单 - Marketplace)
    ↓ Studio accepts
Accepted (已接单 - Studio grabbed)
    ↓ Studio assigns staff
Scheduled (已排期 - Staff assigned)
    ↓ Studio starts shooting [LOCK POINT]
InProgress (拍摄中 - Shooting in progress)
    ↓ Studio publishes delivery
AwaitingConfirmation (待确认 - Delivery submitted)
    ↓ Agent confirms delivery
Completed (已完成 - Order finished)

                OR
    ↓ Agent cancels (only before InProgress)
Cancelled (已取消)
```

### Status Enum

```typescript
export enum OrderStatus {
  Placed = 1,              // Marketplace order, waiting for Studio to accept
  Accepted = 2,            // Studio accepted, can still be cancelled by Agent
  Scheduled = 3,           // Staff assigned, can still be cancelled by Agent
  InProgress = 4,          // Shooting started, order locked (cannot cancel)
  AwaitingConfirmation = 5, // Delivery submitted, waiting for Agent confirmation
  Completed = 6,           // Agent confirmed delivery, order completed
  Cancelled = 7,           // Order cancelled
}
```

---

## 🎬 Order Actions by Role

### Studio Actions

| Status | Action | Next Status | Description |
|--------|--------|-------------|-------------|
| Placed | Accept Order | Accepted | Grab order from marketplace |
| Accepted | Assign Staff & Schedule | Scheduled | Assign photographer/staff |
| Scheduled | Start Shooting | InProgress | Begin shooting, locks order |
| InProgress | (Submit Delivery) | AwaitingConfirmation | Publish delivery package |

### Agent Actions

| Status | Action | Next Status | Description |
|--------|--------|-------------|-------------|
| Placed | Cancel Order | Cancelled | Cancel before Studio accepts |
| Accepted | Cancel Order | Cancelled | Cancel after acceptance |
| Scheduled | Cancel Order | Cancelled | Cancel after scheduling |
| InProgress | ❌ Cannot Cancel | - | Order is locked |
| AwaitingConfirmation | Confirm Delivery | Completed | Accept delivery |
| AwaitingConfirmation | ❌ Cannot Cancel | - | Order is locked |

---

## 📁 Implemented Files

### 1. Order Status Enum
**File**: `frontend/src/types/enums.ts`
- Added `AwaitingConfirmation = 5`
- Updated `Completed = 6`
- Updated `Cancelled = 7`

### 2. Marketplace Page
**File**: `frontend/src/app/dashboard/orders/marketplace/page.tsx`
- Browse all `Placed` orders
- Studio-only access
- Filter and search functionality
- Shows: order details, listing address, tasks, amount

### 3. Order Actions Component
**File**: `frontend/src/components/orders/order-actions.tsx`
- Unified action buttons for all order states
- Role-based rendering (Agent/Studio)
- Confirmation dialogs
- API integration with mutations
- **Actions**:
  - Studio: Accept, Schedule, Start Shooting
  - Agent: Cancel (before InProgress), Confirm Delivery

### 4. Orders API
**File**: `frontend/src/lib/api/orders.ts`
- `acceptOrder(id)` - Studio accepts marketplace order
- `scheduleOrder(id)` - Studio assigns staff
- `startShooting(id)` - Studio starts shooting
- `confirmDelivery(id)` - Agent confirms receipt
- `cancelOrder(id, reason?)` - Agent cancels order

### 5. Order Detail Page
**File**: `frontend/src/app/dashboard/orders/[id]/page.tsx`
- Integrated `OrderActions` component
- Role detection (isAgent, isStudio)
- Shows order timeline
- Links to deliveries when available

### 6. Sidebar Navigation
**File**: `frontend/src/app/dashboard/layout.tsx`
- Studio menu:
  - **Marketplace** → Browse orders
  - **My Orders** → Studio's accepted orders
  - **Deliveries** → Created delivery packages

---

## 🔗 API Endpoints

### Order Management

```typescript
// Studio grabs order from marketplace
POST /api/orders/{id}/accept
  → Status: Placed → Accepted

// Studio assigns staff and schedules
POST /api/orders/{id}/schedule
  → Status: Accepted → Scheduled

// Studio starts shooting (locks order)
POST /api/orders/{id}/start
  → Status: Scheduled → InProgress

// Agent confirms delivery
POST /api/orders/{id}/confirm-delivery
  → Status: AwaitingConfirmation → Completed

// Agent cancels order (before InProgress only)
POST /api/orders/{id}/cancel
  Body: { reason?: string }
  → Status: * → Cancelled
```

### Delivery Integration

```typescript
// Studio publishes delivery
POST /delivery/{id}/publish
  → Automatically updates order status to AwaitingConfirmation
```

---

## 🎨 UI/UX Features

### Marketplace Page (`/dashboard/orders/marketplace`)
- Clean table with order details
- Real-time status badges
- "View Details" button
- Filter by status
- Search functionality

### Order Detail Page (`/dashboard/orders/[id]`)
- Dynamic action buttons based on:
  - Current order status
  - User role (Agent/Studio)
- Status timeline visualization
- Order information cards (Listing, Studio, Tasks)
- Link to deliveries when available

### Order Actions Component
- Context-aware buttons
- Confirmation dialogs with clear descriptions
- Loading states during API calls
- Toast notifications for success/error
- Disabled states when action not available

---

## 🔒 Business Rules

### Cancellation Policy
1. **Agent can cancel**:
   - ✅ When status is `Placed` (before Studio accepts)
   - ✅ When status is `Accepted` (after Studio accepts)
   - ✅ When status is `Scheduled` (after staff assigned)
   - ❌ When status is `InProgress` (shooting started)
   - ❌ When status is `AwaitingConfirmation` (delivery submitted)

2. **Studio cannot cancel** (business decision)
   - Studio must complete accepted orders
   - If issues arise, must be resolved through admin

### Order Locking
- Order is **locked** when shooting starts (`InProgress` status)
- Once locked:
  - Agent cannot cancel
  - Studio must complete the shoot
  - Only way forward is to publish delivery

### Automatic Status Updates
- Publishing a delivery automatically updates order to `AwaitingConfirmation`
- Agent confirming delivery automatically completes the order (`Completed`)

---

## ✅ Completed Features

- [x] OrderStatus enum with AwaitingConfirmation
- [x] Marketplace page for Studio
- [x] Studio accept order functionality
- [x] Studio schedule/assign staff functionality
- [x] Studio start shooting functionality
- [x] Agent confirm delivery functionality
- [x] Agent cancel order (with status restrictions)
- [x] Order actions component (unified)
- [x] Sidebar navigation updates
- [x] Order detail page integration
- [x] Role-based action rendering
- [x] Confirmation dialogs
- [x] API method implementations

---

## 🚧 Pending Features

### Short-term
- [ ] Staff assignment UI (select photographer from dropdown)
- [ ] Order timeline component enhancement
- [ ] Delivery rejection flow (if Agent unhappy with quality)
- [ ] Order notes/comments system

### Medium-term
- [ ] Real-time notifications (WebSocket/SignalR)
  - Order accepted notification → Agent
  - Delivery submitted notification → Agent
  - Order cancelled notification → Studio
- [ ] Email notifications
- [ ] SMS notifications (for shooting reminders)

### Long-term
- [ ] Rating system (Agent rates Studio, Studio rates Agent)
- [ ] Payment integration
- [ ] Dispute resolution system
- [ ] Analytics dashboard (order metrics, studio performance)

---

## 📊 Data Flow

```
Agent creates Order
    ↓
Order published to Marketplace (Placed)
    ↓
Studio accepts Order (Accepted)
    ↓
Studio assigns Staff (Scheduled)
    ↓
Studio starts shooting (InProgress) [LOCK]
    ↓
Studio uploads media to Delivery
    ↓
Studio publishes Delivery (AwaitingConfirmation)
    ↓
Agent views and confirms Delivery
    ↓
Order automatically completed (Completed)
```

---

## 🧪 Testing Checklist

- [x] Studio can view marketplace orders
- [x] Studio can accept order from marketplace
- [x] Studio can schedule order
- [x] Studio can start shooting
- [x] Agent can cancel order before InProgress
- [x] Agent CANNOT cancel order after InProgress
- [x] Agent can confirm delivery
- [x] Order status updates correctly
- [ ] Delivery publish updates order status (needs backend verification)
- [ ] Multiple studios cannot accept same order (needs backend verification)

---

**Implementer**: AI Assistant  
**Review Status**: Ready for testing  
**Deployment Status**: Development environment
