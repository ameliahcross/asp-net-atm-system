using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cajero_MVC.Models;
using Application.Viewmodels;

namespace Cajero_MVC.Controllers;

public class AtmController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Withdrawal()
    {
        return View();
    }

    public IActionResult DispenseMode(int? amount, string dispenseMode)
    {
        var model = new WithdrawViewModel();

        // Si la acción recibe parámetros, asume que el usuario le dio a "Regresar"
        // desde la vista Withdrawal, para seleccionar otro modo de dispensación
        if (amount.HasValue && !string.IsNullOrEmpty(dispenseMode))
        {
            model.Amount = amount.Value;
            model.DispenseMode = dispenseMode;
        }
        // Si no recibe parámetros, está realizando un retiro nuevo. Se carga la vista vacía
        return View(model);
    }

    public IActionResult getAmount()
    {
        return View();
    }

    [HttpPost]
    public IActionResult getAmount(int amount)
    {
        if (!isAmountValid(amount))
        {
            ViewBag.Amount = amount;
            ViewBag.Error = "* Monto inválido: " + errorCause;
            return View("Index");
        }
        else
        {
            ViewBag.Amount = amount;
            return View("DispenseMode");
        }
    }

    string errorCause = "";

    // Recibe el Amount del formulario y valida si es un monto correcto
    public bool isAmountValid(int amount)
    {
        if (amount < 100 || amount % 100 != 0)
        {
            if (amount < 100)
            {
               errorCause = "El monto ingresado no puede ser menor que 100 ";
            }
            else if (amount % 100 != 0){
               errorCause = "El cajero solo dispensa billetes de 100 en adelante";
            }
            return false;
        }
        return true;
    }

    bool withdrawalSucceeded = true;

    [HttpPost] // Cálculo de dispensaciones y resultado de retiro
    public IActionResult Withdrawal(int amount, WithdrawViewModel modeSelection)
    {
        ViewBag.Amount = amount;
        ViewBag.Selection = modeSelection;
        validateAmountAndMode(amount, modeSelection);

        if (!withdrawalSucceeded)
        {
            return View("DispenseMode");
        }
        else
        {  
            int countOf100 = 0;
            int countOf200 = 0;
            int countOf500 = 0;
            int countOf1000 = 0;
            string dispensation = "";
          
            if (modeSelection.DispenseMode == "1")
            {
                countOf500 = amount / 500;
                amount %= 500;
                countOf100 = amount / 100;
                dispensation = "papeletas de $100 y $500";
            }
            else if (modeSelection.DispenseMode == "2")
            {
                countOf1000 = amount / 1000;
                amount %= 1000;
                countOf500 = amount / 500;
                amount %= 500;
                countOf200 = amount / 200;
                amount %= 200;
                countOf100 = amount / 100;
                dispensation = "eficiente";
            }
            else if (modeSelection.DispenseMode == "0")
            {
                countOf1000 = amount / 1000;
                amount %= 1000;
                countOf200 = amount /  200;
                dispensation = "papeletas de $200 y $1000";
            }
            ViewBag.CountOf100 = countOf100;
            ViewBag.CountOf200 = countOf200;
            ViewBag.CountOf500 = countOf500;
            ViewBag.CountOf1000 = countOf1000;
            ViewBag.Dispensation = dispensation;
            return View("Withdrawal");
        }
    }
   
    // Aqui verifico que el modo de dispensación que se elija pueda dispensar billetes para el monto solicitado
    public IActionResult validateAmountAndMode(int amount, WithdrawViewModel modeSelection)
    {
        if (modeSelection.DispenseMode == "0")
        {
            if (amount % 200 == 0 || amount % 1000 == 0)
            {
                ViewBag.Result = "Retiro exitoso";
                return View("Withdrawal");
            }
            else
            {
                ViewBag.Result = "* Este cajero solo dispensa papeletas de 200 y 1000. Seleccione otro modo.";
                withdrawalSucceeded = false;
                return View("DispenseMode");
            }
        }
        else if (modeSelection.DispenseMode == "1" || modeSelection.DispenseMode == "2")
        {
            if (amount % 100 == 0)
            {
                ViewBag.Result = "Retiro exitoso";
                return View("Withdrawal");
            }
            else
            {
                ViewBag.Result = "* Este cajero solo dispensa papeletas de 100 y 500. Seleccione otro modo.";
                withdrawalSucceeded = false;
                return View("DispenseMode");
            }
        }
        return View("Withdrawal");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

