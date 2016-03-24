//Code
$(document).ready(function () {   
    $("#Button1").click(function () {
        $("#result").empty();
        $("#jugando").empty();        
        WebService1.Service1.send($("#txtData").val(), success, error);

        $("#jugando").fadeIn();
        $("#jugando").append("<span class='cierre'>X</span>");
    });

    $(function () {        
        $("#jugando").draggable();
    });
    $("#jugando").on("click", ".cierre", function () {
        $("#jugando").fadeOut('slow');
    });

    $("#Match").on("click", "#Button2", function () {
        $("#dialog").dialog({ 
        width: 590,  
        height: 350,
        show: "scale", 
        hide: "scale",
        resizable: "false",
        modal: "true"
        });
    });
})

function rectanguloSimple(texto)
{
    // Crear canvas 420 × 200 
    //mostrare los resultados en el div result
    var contenedor = $("#result")[0];
    var lienzo = Raphael(contenedor, 420, 200);
    var rectangulo = lienzo.rect(20, 30, 28, 24, 3, 3).attr({ fill: "#dae1e1" });
    lienzo.text(34, 41, texto).attr({ 'font-size': 14 });
    lienzo.path("M10,42H20").attr({ "stroke-width": 2 });
    lienzo.circle(6, 42, 5).attr({ fill: "#6b6659", "stroke-width": 2 });

    //final del dibujo
    lienzo.path("M58,42H48").attr({ "stroke-width": 2 });
    lienzo.circle(63, 42, 5).attr({ fill: "#6b6659", "stroke-width": 2 });


    //esto es un rectangulo con lineas discontinuas para los grupos
    //lienzo.rect(50, 80, 28, 24, 3, 3).attr({ 'stroke-dasharray': '--' });
}

//funcion addInput que genera un input nuevo en caso de que no haya error en la expresion para hacer los macth
function addInput()
{
    if (wrong) {
        if ($('#secondInput').length) {
            $("#Match").fadeOut();
        }
    }
    else
    {
        if ($('#secondInput').length) {
            //Ejecutar si existe el elemento
            $("#Match").fadeIn();
        }
        else
        {
            $("#Match").append("<input type='text' id='secondInput' placeholder='Match' name='name'><input id='Button2' type='button' value='Match'/><br/>");
           }
    }
    wrong = false;
}

function add(elemento)
{
    $("#jugando").append(elemento);
}

function success(response) {
    analizar(response, pref);
    addInput();
}

//VARIABLES GLOBALES
var saltoLinea = '<br/>';
var tabulador = "<span id='tab1'></span>";
pref = "";
var wrong = false;

function analizar(response, pref)
{
    if (response.Tipo == -1)
    {
        wrong = true;
    }
    if (response.Tipo == 1 || response.Tipo == -1)
    {
        if (response.Repeticiones == "" || response.Repeticiones == null)
        {
            rectanguloSimple(response.Valor);
            add(pref + response.Valor + saltoLinea);
        }
        else
        {
            add(pref + response.Valor + "-" + response.Repeticiones + saltoLinea);
        }
    }
    if (response.Tipo == 9)
    {
        add(pref + response.Valor + saltoLinea);
    }
    else
    {
        if (response.Tipo == 2) add(pref + '(' + saltoLinea);
        if (response.Tipo == 4)
        {
            if (response.Subtipo == 1) {
                add(pref + '[' + saltoLinea + pref + 'Ninguno de' + saltoLinea);
            }
            else
            {
                add(pref + '[' + saltoLinea);
            }
        }
        var count = 0;
        $.each(response.Componentes, function () {
            analizar(this, pref + tabulador);
            count++;
            if ((response.Tipo == 3) && (count < response.Componentes.length)) {
                add(pref+'|' + saltoLinea);
            }                  
        });

        if (response.Tipo == 2)
        {
            if (response.Repeticiones == null) {
                add(pref+')' + saltoLinea);
                $("#result").append();
            }
            else { add(pref + ')' + response.Repeticiones + saltoLinea); }
        }            
        if (response.Tipo == 4) add(pref + ']' + response.Repeticiones + saltoLinea);
    }
}
function error(response) {
    add(response);
}