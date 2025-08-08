using KidZone.API.Data;
using KidZone.API.DTOs;
using KidZone.Domain.Entities;
using KidZone.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KidZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public SubscriptionController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("Create-Plans")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreatePlan(CreateSubscriptionPlanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var plan = new SubscriptionPlan
            {
                Name = dto.Name,
                Price = dto.Price,
                DurationInMonths = dto.DurationInMonths

            };

            _context.SubscriptionPlans.Add(plan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlanById), new { id = plan.ID }, plan);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
                return NotFound();

            return Ok(plan);
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _context.SubscriptionPlans.ToListAsync();
            return Ok(plans);
        }

        [HttpPost("subscribe/{planId}")]
        public async Task<IActionResult> Subscribe(int planId)
        {
            var user = await _userManager.GetUserAsync(User);

            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null)
                return NotFound("Subscription plan not found");

            var activeSub = await _context.UserSubscriptions
                .FirstOrDefaultAsync(s => s.UserID == user.Id && s.Status == SubscriptionStatus.Active);

            if (activeSub != null)
            {
                activeSub.Status = SubscriptionStatus.Cancelled;
                _context.UserSubscriptions.Update(activeSub);
            }

            var subscription = new UserSubscription
            {
                UserID = user.Id,
                Sub_Plan_ID = planId,
                Status = SubscriptionStatus.Active,
                Start_Date = DateTime.UtcNow,
                End_Date = DateTime.UtcNow.AddMonths(plan.DurationInMonths),
                Payments = new List<Payment>()
            };

            _context.UserSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Subscription created successfully", subscriptionId = subscription.ID });
        }

        
        [HttpGet("MySubscriptions")]
        public async Task<IActionResult> GetMySubscriptions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var subscriptions = await _context.UserSubscriptions
                .Include(s => s.Plan)
                .Where(s => s.UserID == userId)
                .Select(s => new
                {
                    s.ID,
                    PlanName = s.Plan.Name,
                    s.Plan.Price,
                    s.Plan.DurationInMonths
                })
                .ToListAsync();

            return Ok(subscriptions);
        }


        [HttpPost("create-SubScriptionWithPayment")]
        public async Task<IActionResult> CreateSubscriptionWithPayment(SubscriptionRequestDto dto)
        {
            var user = await _userManager.GetUserAsync(User);

            var plan = await _context.SubscriptionPlans.FindAsync(dto.PlanId);
            if (plan == null)
                return NotFound("Subscription plan not found");

            var activeSub = await _context.UserSubscriptions
                .FirstOrDefaultAsync(s => s.UserID == user.Id && s.Status == SubscriptionStatus.Active);

            if (activeSub != null)
            {
                activeSub.Status = SubscriptionStatus.Cancelled;
                _context.UserSubscriptions.Update(activeSub);
            }

            var subscription = new UserSubscription
            {
                UserID = user.Id,
                Sub_Plan_ID = plan.ID,
                Status = SubscriptionStatus.Pending, 
                Start_Date = DateTime.UtcNow,
                End_Date = DateTime.UtcNow.AddMonths(plan.DurationInMonths),
                Payments = new List<Payment>()
            };

            _context.UserSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            var payment = new Payment
            {
                Amount = plan.Price,
                Method = dto.PaymentMethod,
                Status = PaymentStatus.Pending,
                Date = DateTime.UtcNow,
                User_Sub_ID = subscription.ID
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            bool paymentSuccess = true; 

            if (paymentSuccess)
            {
                payment.Status = PaymentStatus.Completed;
                subscription.Status = SubscriptionStatus.Active;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                subscription.Status = SubscriptionStatus.Cancelled;
            }

            _context.Payments.Update(payment);
            _context.UserSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = paymentSuccess ? "Subscription activated successfully" : "Payment failed, subscription cancelled",
                subscriptionId = subscription.ID,
                paymentId = payment.ID,
                paymentStatus = payment.Status.ToString(),
                subscriptionStatus = subscription.Status.ToString()
            });
        }

        [HttpGet("check-status")]
        public async Task<IActionResult> CheckSubscriptionStatus()
        {
            var user = await _userManager.GetUserAsync(User);

            var subscription = await _context.UserSubscriptions
                .Include(s => s.Plan)
                .Include(s => s.Payments)
                .Where(s => s.UserID == user.Id)
                .OrderByDescending(s => s.Start_Date)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return Ok(new { hasSubscription = false, message = "No active or past subscriptions found." });

            var lastPayment = subscription.Payments
                .OrderByDescending(p => p.Date)
                .FirstOrDefault();

            return Ok(new
            {
                hasSubscription = true,
                subscriptionId = subscription.ID,
                planName = subscription.Plan?.Name,
                planPrice = subscription.Plan?.Price,
                status = subscription.Status.ToString(),
                startDate = subscription.Start_Date,
                endDate = subscription.End_Date,
                lastPayment = lastPayment != null ? new
                {
                    paymentId = lastPayment.ID,
                    amount = lastPayment.Amount,
                    method = lastPayment.Method.ToString(),
                    status = lastPayment.Status.ToString(),
                    date = lastPayment.Date
                } : null
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdatePlan(int id, CreateSubscriptionPlanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
                return NotFound(new { Message = "Subscription plan not found" });

            plan.Name = dto.Name;
            plan.Price = dto.Price;
            plan.DurationInMonths = dto.DurationInMonths;

            _context.SubscriptionPlans.Update(plan);
            await _context.SaveChangesAsync();

            return Ok(plan);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
                return NotFound(new { Message = "Subscription plan not found" });

            _context.SubscriptionPlans.Remove(plan);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
