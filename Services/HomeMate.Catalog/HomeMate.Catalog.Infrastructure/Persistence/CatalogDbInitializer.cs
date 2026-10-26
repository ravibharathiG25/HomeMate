using HomeMate.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeMate.Catalog.Infrastructure.Persistence;

public static class CatalogDbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CatalogDbContext>>();

        try
        {
            logger.LogInformation("Ensuring HomeMateCatalogDB is created...");
            await context.Database.EnsureCreatedAsync();

            if (await context.Categories.AnyAsync())
            {
                logger.LogInformation("Catalog database already contains seeded data.");
                return;
            }

            logger.LogInformation("Seeding initial categories and services...");

            // 1. Cleaning & Disinfection
            var cleaning = Category.Create(
                name: "Cleaning & Disinfection",
                slug: "cleaning",
                description: "Professional home, kitchen, bathroom, and upholstery deep cleaning solutions.",
                icon: "sparkles",
                imageUrl: "https://images.unsplash.com/photo-1581578731548-c64695cc6952?auto=format&fit=crop&w=800&q=80",
                displayOrder: 1);

            cleaning.AddService(ServiceItem.Create(
                cleaning.Id,
                name: "Full Home Deep Cleaning",
                slug: "full-home-deep-cleaning",
                shortDescription: "Complete floor scrubbing, window cleaning, dusting, and sanitization for up to 3BHK.",
                description: "Our certified cleaning experts thoroughly clean and disinfect every corner of your house. Includes high-pressure scrubbing, eco-friendly chemical treatment, kitchen degreasing, bathroom descaling, and balcony wash.",
                price: 129.99m,
                originalPrice: 159.99m,
                durationMinutes: 240,
                imageUrl: "https://images.unsplash.com/photo-1581578731548-c64695cc6952?auto=format&fit=crop&w=800&q=80",
                isFeatured: true,
                rating: 4.9,
                totalReviews: 128));

            cleaning.AddService(ServiceItem.Create(
                cleaning.Id,
                name: "Bathroom & Tile Descaling",
                slug: "bathroom-tile-descaling",
                shortDescription: "Deep scrub, mirror polishing, and grout grime removal for spotless bathrooms.",
                description: "Targeted hard-water stain removal, floor and wall tile restoration, tap buffing, and antibacterial steam spray for hygiene.",
                price: 49.99m,
                originalPrice: 65.00m,
                durationMinutes: 90,
                imageUrl: "https://images.unsplash.com/photo-1584622650111-993a426fbf0a?auto=format&fit=crop&w=800&q=80",
                isFeatured: false,
                rating: 4.8,
                totalReviews: 84));

            cleaning.AddService(ServiceItem.Create(
                cleaning.Id,
                name: "Sofa & Carpet Shampooing",
                slug: "sofa-carpet-shampooing",
                shortDescription: "Industrial fabric vacuuming and foam shampoo for living room furniture.",
                description: "Eliminate allergens, dust mites, and stubborn stains from sofas and rugs using dry foam extraction.",
                price: 59.99m,
                originalPrice: 75.00m,
                durationMinutes: 90,
                imageUrl: "https://images.unsplash.com/photo-1556911220-e15b29be8c8f?auto=format&fit=crop&w=800&q=80",
                isFeatured: false,
                rating: 4.7,
                totalReviews: 46));

            // 2. Plumbing Services
            var plumbing = Category.Create(
                name: "Plumbing Services",
                slug: "plumbing",
                description: "Reliable pipe repairs, tap fixes, drain declogging, and sanitary installations.",
                icon: "wrench",
                imageUrl: "https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?auto=format&fit=crop&w=800&q=80",
                displayOrder: 2);

            plumbing.AddService(ServiceItem.Create(
                plumbing.Id,
                name: "Tap, Valve & Pipe Leak Repair",
                slug: "tap-pipe-leak-repair",
                shortDescription: "Quick fixing of leaking mixers, diverters, pipe connections, and valves.",
                description: "Prompt diagnosis and precision repair of dripping taps, fractured PVC/CPVC pipes, and joint leaks to avoid water wastage.",
                price: 39.99m,
                originalPrice: 49.99m,
                durationMinutes: 45,
                imageUrl: "https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?auto=format&fit=crop&w=800&q=80",
                isFeatured: true,
                rating: 4.9,
                totalReviews: 215));

            plumbing.AddService(ServiceItem.Create(
                plumbing.Id,
                name: "Severe Drain Blockage Removal",
                slug: "severe-drain-blockage-removal",
                shortDescription: "Mechanical motorized snake clear-out for blocked sinks, showers, and gully traps.",
                description: "Fast-acting unclogging with rotary cable equipment that eliminates grease, hair, and soap scum buildup without damaging pipes.",
                price: 54.99m,
                originalPrice: 69.99m,
                durationMinutes: 60,
                imageUrl: "https://images.unsplash.com/photo-1542013936693-884638332954?auto=format&fit=crop&w=800&q=80",
                isFeatured: false,
                rating: 4.8,
                totalReviews: 92));

            // 3. Electrical & Wiring
            var electrical = Category.Create(
                name: "Electrical & Wiring",
                slug: "electrical",
                description: "Certified electricians for fan installs, breaker diagnostics, and wiring repairs.",
                icon: "bolt",
                imageUrl: "https://images.unsplash.com/photo-1621905251189-08b45d6a269e?auto=format&fit=crop&w=800&q=80",
                displayOrder: 3);

            electrical.AddService(ServiceItem.Create(
                electrical.Id,
                name: "Switchboard & MCB Troubleshooting",
                slug: "switchboard-mcb-troubleshooting",
                shortDescription: "Safe replacement of sparking switches, blown fuses, and tripping breakers.",
                description: "Comprehensive testing of load limits, short circuits, earthing checks, and rewiring of faulty distribution boxes.",
                price: 44.99m,
                originalPrice: 59.99m,
                durationMinutes: 60,
                imageUrl: "https://images.unsplash.com/photo-1621905251189-08b45d6a269e?auto=format&fit=crop&w=800&q=80",
                isFeatured: true,
                rating: 4.9,
                totalReviews: 174));

            electrical.AddService(ServiceItem.Create(
                electrical.Id,
                name: "Ceiling Fan & Chandelier Fitting",
                slug: "ceiling-fan-chandelier-fitting",
                shortDescription: "Secure mounting and assembly of decorative lights, smart fans, and fixtures.",
                description: "Includes structural rod anchoring, blade balancing, wireless remote control configuration, and wiring insulation.",
                price: 34.99m,
                originalPrice: 45.00m,
                durationMinutes: 45,
                imageUrl: "https://images.unsplash.com/photo-1565814636199-ae8133055c1c?auto=format&fit=crop&w=800&q=80",
                isFeatured: false,
                rating: 4.8,
                totalReviews: 110));

            // 4. Appliance Care & AC
            var appliance = Category.Create(
                name: "Appliance Care & AC",
                slug: "appliance-care",
                description: "Expert servicing for air conditioners, refrigerators, washing machines, and microwaves.",
                icon: "cpu",
                imageUrl: "https://images.unsplash.com/photo-1581092918056-0c4c3acd3789?auto=format&fit=crop&w=800&q=80",
                displayOrder: 4);

            appliance.AddService(ServiceItem.Create(
                appliance.Id,
                name: "Split AC Deep Power Jet Servicing",
                slug: "split-ac-power-jet-servicing",
                shortDescription: "High-pressure jet wash of cooling coils, filter sanitization, and gas pressure check.",
                description: "Improves cooling efficiency by up to 35% and reduces energy consumption. Includes drain pipe flushing and outdoor condenser wash.",
                price: 69.99m,
                originalPrice: 89.99m,
                durationMinutes: 75,
                imageUrl: "https://images.unsplash.com/photo-1621905251918-48416bd8575a?auto=format&fit=crop&w=800&q=80",
                isFeatured: true,
                rating: 4.9,
                totalReviews: 340));

            appliance.AddService(ServiceItem.Create(
                appliance.Id,
                name: "Washing Machine Diagnostic & Tune-up",
                slug: "washing-machine-tune-up",
                shortDescription: "Vibration dampening, pump cleaning, and error code fixes for front/top load units.",
                description: "Addresses water intake problems, spinning drum noise, motor belt slip, and electronic PCB faults.",
                price: 49.99m,
                originalPrice: 65.00m,
                durationMinutes: 60,
                imageUrl: "https://images.unsplash.com/photo-1626806787461-102c1bfaaea1?auto=format&fit=crop&w=800&q=80",
                isFeatured: false,
                rating: 4.7,
                totalReviews: 76));

            // 5. Painting & Waterproofing
            var painting = Category.Create(
                name: "Painting & Waterproofing",
                slug: "painting",
                description: "Interior wall design, dampness treatment, and exterior weatherproofing.",
                icon: "paint-brush",
                imageUrl: "https://images.unsplash.com/photo-1562259949-e8e7689d7828?auto=format&fit=crop&w=800&q=80",
                displayOrder: 5);

            painting.AddService(ServiceItem.Create(
                painting.Id,
                name: "Wall Dampness & Anti-Fungal Treatment",
                slug: "wall-dampness-treatment",
                shortDescription: "Deep moisture barrier injection and elastomeric waterproofing coating.",
                description: "Stops peeling paint, salt efflorescence, and toxic black mold with commercial-grade sealant membranes.",
                price: 119.99m,
                originalPrice: 149.99m,
                durationMinutes: 180,
                imageUrl: "https://images.unsplash.com/photo-1562259949-e8e7689d7828?auto=format&fit=crop&w=800&q=80",
                isFeatured: true,
                rating: 4.8,
                totalReviews: 63));

            // 6. Carpentry & Assembly
            var carpentry = Category.Create(
                name: "Carpentry & Assembly",
                slug: "carpentry",
                description: "Flat-pack furniture assembly, door locks, hinges, and custom woodwork fixes.",
                icon: "hammer",
                imageUrl: "https://images.unsplash.com/photo-1533090161767-e6ffed986c88?auto=format&fit=crop&w=800&q=80",
                displayOrder: 6);

            carpentry.AddService(ServiceItem.Create(
                carpentry.Id,
                name: "Smart Door Lock & Handle Installation",
                slug: "smart-door-lock-installation",
                shortDescription: "Precision chisel mortise fitting for digital fingerprint and keypad smart locks.",
                description: "Clean door cutting, strike plate alignment, cable routing, and mobile app pairing for solid wood and metal doors.",
                price: 49.99m,
                originalPrice: 65.00m,
                durationMinutes: 60,
                imageUrl: "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&w=800&q=80",
                isFeatured: false,
                rating: 4.9,
                totalReviews: 88));

            await context.Categories.AddRangeAsync(cleaning, plumbing, electrical, appliance, painting, carpentry);
            await context.SaveChangesAsync();

            logger.LogInformation("Successfully seeded {Count} categories and services into HomeMateCatalogDB.", 6);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to seed Catalog database.");
            throw;
        }
    }
}
