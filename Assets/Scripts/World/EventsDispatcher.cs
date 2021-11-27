using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class EventsDispatcher
{
    public static event System.Action inventoryUpdated;

    public static void sendInventoryUpdated()
    {
        inventoryUpdated?.Invoke();
    }
}
