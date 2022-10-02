using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleCQRS.Core;

namespace SimpleCQRS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var bus = new FakeBus();
            var storage = new EventStore(bus);
            var rep = new Repository<InventoryItem>(storage);
            var commands = new InventoryCommandHandlers(rep);
            bus.RegisterHandler<CheckInItemsToInventory>(commands.Handle);
            bus.RegisterHandler<CreateInventoryItem>(commands.Handle);
            bus.RegisterHandler<DeactivateInventoryItem>(commands.Handle);
            bus.RegisterHandler<RemoveItemsFromInventory>(commands.Handle);
            bus.RegisterHandler<RenameInventoryItem>(commands.Handle);
            var detail = new InventoryItemDetailView();
            bus.RegisterHandler<InventoryItemCreated>(detail.Handle);
            bus.RegisterHandler<InventoryItemDeactivated>(detail.Handle);
            bus.RegisterHandler<InventoryItemRenamed>(detail.Handle);
            bus.RegisterHandler<ItemsCheckedInToInventory>(detail.Handle);
            bus.RegisterHandler<ItemsRemovedFromInventory>(detail.Handle);
            var list = new InventoryListView();
            bus.RegisterHandler<InventoryItemCreated>(list.Handle);
            bus.RegisterHandler<InventoryItemRenamed>(list.Handle);
            bus.RegisterHandler<InventoryItemDeactivated>(list.Handle);
            ServiceLocator.Bus = bus;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
