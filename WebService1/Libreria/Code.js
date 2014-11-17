//VARIABLES GLOBALES
var saltoLinea = '<br/>';
var tabulador = "<span id='tab1'></span>";
pref = "";
var wrong = false;
var desp = 0;
var lienzoEjeY = 0;
var lienzoEjeX = 0;
var cantidadGrupos = 0;
var var_cant = 0;
var cx = 20;
var cy = 0;
var centroLienzo = 0;
var currentExp = "";

var idiomaElegido = idioma();
//Funcion para detecar el idioma del navegador
function idioma()
{
    var navegador = navigator.language;
    if (navegador == undefined)
    {
        navegador = navigator.userLanguage;
    }    
    var interna = navegador.indexOf('-', 0);
    if (interna == -1)
    {
        return navegador;
    }
    return navegador.substr(0,interna);
}

/*Funcion que añadre un script con el lenguaje del navegador*/
function include(file_path) {
    var j = document.createElement("script");
    j.src = file_path;
    j.type = "text/javascript";
    document.head.appendChild(j);
}

//Comprueba si existe msg y si existe el texto. Si es así, devuelve ese texto. En caso contrario, devuelve lo mismo que se le pasa.
function comprobar(s) {
    if (typeof (msg) != 'undefined' && msg[s]) {
        return msg[s];
    }
    return s;
}

$(document).ready(function () {
    //verificar que el text area tiene algo antes de realizar nada
    $("#txtData").focusout(function () {
        if ($("#txtData").val().length == 0) {
            $("#Button1").prop("disabled", true);
        }
        else { $("#Button1").prop("disabled", false); }
    });
    $("#txtData").keyup(function () {
        if ($("#txtData").val().length != 0)
        {
            $("#Button1").prop("disabled", false);
            if ($('#secondInput').length)
            {
                if (currentExp != $("#txtData").val())
                {
                    $("#Match").fadeOut();
                    $("#expresion").fadeOut();
                    $("#result").fadeOut();
                    $("#same").fadeOut();
                }
                else
                {
                    if (!wrong) { $("#Match").fadeIn(); }
                    $("#expresion").fadeIn();
                    $("#result").fadeIn();
                    $("#same").fadeIn();
                }
            }
        }
        else {
            $("#Button1").prop("disabled", true);
            $("#Match").fadeOut();
            $("#expresion").fadeOut();
            $("#result").fadeOut();
            $("#same").fadeOut();
        }
    });

    //Funcion que carga el js para el lenguaje
    include("idiomas/" + idiomaElegido + ".js");

    $("#Button1").click(function () {
        wrong = false;
        $("#result").fadeIn();
        $("#same").fadeIn();
        //Restaurar valores predeterminados
        cantidadGrupos = 0;
        var_cant = 0;
        $("#result").empty();
        $("#same").empty();
        $('#expresion').empty();
        WebService1.Service1.analizar($("#txtData").val(), success, error);

        //llamar a funcion que agrega la expresion al div expresion donde se pondra la tabla que tiene los match
        addDivExpresion();
        document.getElementById("result").style.overflowX = "";        
        $("#expresion").css('position', '');
        currentExp = $("#txtData").val();
    });

    $("#Match").on("click", "#Button2", function () {
        $("#expresion").empty();
        $("#expresion").show();
        WebService1.Service1.analizar_match($("#txtData").val(), $("#secondInput").val(), successMatch, error);
    });

    $("#secondInput").focus();

    $("#expresion").on("click", "#close", function () {
        $("#expresion").hide();
        $("#expresion").css('position', '');
    });

    $("#expresion").on("mouseover", "#close", function () {
        $(this).css('cursor', 'pointer');
    });
});

function addDivExpresion() {
    $('#expresion').show();
}

/*funcion para calcular el tamaño del rectangulo a dibujar dependiendo del string a contener del font y del font-size
font = 12
fontSize = times*/
function tam_rectangulo(string, font, fontSize)
{
    var paper = Raphael(0, 0, 0, 0);
    paper.canvas.style.visibility = 'hidden';
    var el = paper.text(0, 0, string);
    el.attr('font-family', font);
    el.attr('font-size', fontSize);
    var bBox = el.getBBox();
    paper.remove();
    return bBox.width;
}

//funcion que contiene un numero relativo del cual empezar a calcular en el eje Y
function largo()
{
    return 24;
}

/*funcion addInput que genera un input nuevo en caso de que no haya error en la expresion para hacer los macth
y ademas en caso de que la expresion contenga \ las dobla y las muestra.
*/
function addInput()
{
    if (wrong) {        
            $("#Match").fadeOut();
    }
    else
    {
        if ($('#secondInput').length == 0) {
            $("#Match").append("<input type='text' id='secondInput' placeholder='" + comprobar("match") + "' name='name'><input id='Button2' type='button' value='" + comprobar("match") + "'/><br/>");
        }
        $("#Match").fadeIn();
        doble_barra($('#txtData').val());
    }
}

/*funcion que para el caso tipo 3 arregla los elementos para que queden todos ajustados
para cada componente de actual restar a su cy el valor pasado por parametro
si ese componente no es simple recursivo*/
function restarCy (actual, valor)
{
    actual.cy = actual.cy - valor;
    if (actual.Tipo != 1 && actual.Tipo != -1) {
        for (var i = 0; i < actual.Componentes.length; i++) {
            var tomar = actual.Componentes[i];
            restarCy(tomar, valor);
        }
    }
}

//Funcion que calcula para cada RegexItem su posicion donde tiene que ir
function calcularCoordenadas(response, cx, cy) {
    response.cx = cx;
    if (response.Tipo == 2 && response.Subtipo == 0)
    {
        var_cant = var_cant + 1;
    }
    if (response.Subtipo == 300)
    {
        if (response.Valor > var_cant)
        {
            response.Subtipo = 301;
        }
        response.Valor = comprobar("Referencia") + response.Valor + ")";
    }
    if (response.Tipo == 9 || response.Tipo == 1 || response.Tipo == -1) {
        response.cy = cy - largo() / 2;
        response.width = (parseInt(tam_rectangulo(comprobar(response.Valor), 'Times', 14)) + parseInt(21));
        response.height = largo();                 
    }
    else
    {
        if (response.Tipo == 3)
        {
            var count = 0;
            var localCx = cx;
            var localCy = cy - ((response.Componentes.length * largo() + (response.Componentes.length - 1) * 10) / 2) + largo() / 2;
            var localHeigth = 0;
            var localWidth = 0;
            for (var i = 0; i < response.Componentes.length; i++) {
                var actual = response.Componentes[i];
                calcularCoordenadas(actual, localCx + 10, localCy);

                localCy = actual.cy + actual.height + 20;
                localHeigth += actual.height + 10;
                if (actual.width > localWidth) localWidth = actual.width;
            }
            /*para volver a colocar bien cy*/
            for (var i = response.Componentes.length - 2; i >= 0; i--) {
                var actual = response.Componentes[i];
                var siguiente = response.Componentes[i + 1];
                var final = actual.cy + actual.height;
                if (final >= siguiente.cy) 
                {
                    valor_a_restar = (final - siguiente.cy + 10);
                    restarCy(actual, valor_a_restar);
                }
            }           
            var ultimo = response.Componentes[response.Componentes.length - 1];
            response.width = localWidth + 20;
            response.height = localHeigth + 20;
            response.cy = cy - response.height / 2;
        }
        else
        {
            if (response.Tipo == 4) {
                var count = 0;
                var localCx = cx;
                var localCy = cy - ((response.Componentes.length * largo() + (response.Componentes.length - 1) * 10) / 2) + largo() / 2;
                var localHeigth = 0;
                var localWidth = 0;
                for (var i = 0; i < response.Componentes.length; i++) {
                    var actual = response.Componentes[i];
                    calcularCoordenadas(actual, localCx + 10, localCy);
                    localCy = actual.cy + actual.height + 20;
                    localHeigth += actual.height + 10;
                    if (actual.width > localWidth) localWidth = actual.width;
                }
                var ultimo = response.Componentes[response.Componentes.length - 1];
                response.width = localWidth + 20;
                response.height = localHeigth + 20;
                response.cy = cy - response.height / 2;
            }
            else //Tipo 2 y 0
            {
                var count = 0;
                var localCx = cx;
                var localCy = cy;
                var localHeigth = 0;
                if (response.Componentes.length > 0 && response.Componentes[0].Repeticiones != "") {
                    localCx = localCx + 10;
                }
                for (var i = 0; i < response.Componentes.length; i++) {
                    var actual = response.Componentes[i];
                    /*En este punto se tiene en cuenta si el elemento tiene repeticiones entonces sumar mas al elto localCx*/
                    if (actual.Repeticiones != null && actual.Repeticiones != "") {
                        calcularCoordenadas(actual, localCx + 20, localCy);
                    }
                    else { calcularCoordenadas(actual, localCx + 10, localCy); }                    
                    localCx = actual.cx + actual.width + 10;  /*+10 para que se separe un poco el sgte elto*/
                    var inc = (actual.Repeticiones != "") ? 30 : 10;
                    if (actual.height + inc > localHeigth) {
                        localHeigth = actual.height + inc;
                    }
                }
                var ultimo;
                if (response.Componentes.length > 0) {
                    ultimo = response.Componentes[response.Componentes.length - 1];
                }
                else { ultimo = {cx : response.cx, width : 0}}
                var inc = (ultimo.Repeticiones != "") ? 20 : 10;
                response.width = (ultimo.cx + ultimo.width) - response.cx + inc;
                /*Si es tipo 3 el componente de grupo y sumar +10*/
                if (response.Componentes.length == 1 && response.Componentes[0].Tipo == 3) {
                    response.height = response.Componentes[0].height;
                }
                else { response.height = localHeigth + 20; }
                response.cy = cy - response.height / 2;
            }
        }
    }
    lienzoEjeX = response.width + 65;
    lienzoEjeY = response.height + 10;
    /*Comprobar que si es muy grande el tamaño a dibujar para colocar, scroll al SVG*/
    if (lienzoEjeX > 1285.1999999999998)
    {
        lienzoEjeX = lienzoEjeX + 35; //Añado un poco mas para luego pintar el circulo final del automata
        document.getElementById("result").style.overflowX = "scroll";
    }
}

//funcion para comprobar en caso de que sea un metacaracter para dibujarlo en verde en rectangulo
function metaCaracter(response)
{
    if (response.Subtipo >=151 && response.Subtipo <= 299) {
        return true;
    }
    else
    {
        return false;
    }
}

//funciones que se encargan del parpadeo del error
function parpadeo(rec)
{
    function rojo() {
        rec.animate({ fill: '#9B2727' }, 1000, blanco);
    }
    function blanco() {
        rec.animate({ r: 6, fill: '#dae1e1' }, 1000, rojo);
    }
    rojo();
}

//funcion que dibuja usando las coordenas calculadas anteriormente
function dibujar(response, lienzo)
{
    if (response.Tipo == -1) 
    {
        wrong = true;
        var rect = lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ fill: "#dae1e1" });
        lienzo.text((parseInt(response.cx) + parseInt(response.width / 2)), response.cy + centroLienzo + 10, comprobar(response.Valor)).attr({ 'font-size': 14, 'font-family': "Times" });       
        parpadeo(rect);
    }
    if (response.Tipo == 1 || response.Tipo == 9)
    {
        if (metaCaracter(response))
        {
            lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ fill: "#bada55", stroke: "#bada55" });
            lienzo.text((parseInt(response.cx) + parseInt(response.width / 2)), response.cy + centroLienzo + 10, comprobar(response.Valor)).attr({ 'font-size': 12, 'font-family': "Times" });
        }
        else
        {
            if (response.Subtipo == 15) {
                lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ fill: "#6b6659", stroke: "#6b6659" });
                lienzo.text((parseInt(response.cx) + parseInt(response.width / 2)), response.cy + centroLienzo + 10, comprobar(response.Valor)).attr({ fill: "#ffffff", 'font-size': 14, 'font-family': "Times" });
            }
            //Referencia de grupo y el grupo existe
            if (response.Subtipo == 300)
            {
                lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ fill: "#00B233", stroke: "#00B233" });
                lienzo.text((parseInt(response.cx) + parseInt(response.width / 2)), response.cy + centroLienzo + 10, comprobar(response.Valor)).attr({'font-size': 14, 'font-family': "Times" });
            }
            //Referencia de grupos que no estan
            if (response.Subtipo == 301)
            {
                wrong = true;
                var rectangulo = lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ fill: "#00B233", stroke: "#00B233" });
                lienzo.text((parseInt(response.cx) + parseInt(response.width / 2)), response.cy + centroLienzo + 10, comprobar(response.Valor)).attr({ 'font-size': 14, 'font-family': "Times" });
                parpadeo(rectangulo);
            }
            else
            {
                if (response.Tipo == 9) {
                    lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ fill: "#49CD95" });
                    lienzo.text((parseInt(response.cx) + parseInt(response.width / 2)), response.cy + centroLienzo + 10, comprobar(response.Valor)).attr({ 'font-size': 14, 'font-family': "Times" });
                }
                else if(response.Tipo == 1 && response.Subtipo != 300 && response.Subtipo != 15) {
                    lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ fill: "#dae1e1" });
                    lienzo.text((parseInt(response.cx) + parseInt(response.width / 2)), response.cy + centroLienzo + 10, comprobar(response.Valor)).attr({ 'font-size': 14, 'font-family': "Times" });
                }
            }            
        }
    }
    else
    {
        if (response.Tipo == 2 && response.Subtipo == 0) {
            lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ 'stroke-dasharray': '--' });
            cantidadGrupos = cantidadGrupos + 1;
            lienzo.text(response.cx + 20, response.cy + centroLienzo - 6, comprobar("Grupo") + cantidadGrupos).attr({ 'font-size': 10 });
        }
        if (response.Tipo == 2 && response.Subtipo == 1)
        {
            lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ 'stroke-dasharray': '--' });
            lienzo.text(response.cx + 48, response.cy + centroLienzo - 6, comprobar("GrupoNegativo")).attr({ 'font-size': 10 });
        }
        else
        {
            if (response.Tipo == 4)
            {
                if (response.Subtipo == 1)
                {
                    lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ fill: "#cbcbba", stroke: "#cbcbba" });
                    lienzo.text(response.cx + 26, response.cy + centroLienzo - 6, comprobar("Ninguno_de")).attr({ 'font-size': 10 });
                }
                else
                {
                    lienzo.rect(response.cx, response.cy + centroLienzo, response.width, response.height, 3, 3).attr({ fill: "#cbcbba", stroke: "#cbcbba" });
                    lienzo.text(response.cx + 26, response.cy + centroLienzo - 6, comprobar("Uno_de")).attr({ 'font-size': 10 });
                }                
            }
        }
        $.each(response.Componentes, function () {
            dibujar(this, lienzo);
        });
    }
}

/*Funcion que dibuja el circulo inicial y final*/
function dibujar_inicio_fin(response, lienzo)
{
    lienzo.circle((parseInt(response.cx) - parseInt(5)), centroLienzo, 5).attr({ fill: "#6b6659", stroke: "#000", 'stroke-width': "2" });
    lienzo.circle((parseInt(response.cx) + parseInt(response.width) + parseInt(5)), centroLienzo, 5).attr({ fill: "#6b6659", stroke: "#000", 'stroke-width': "2" });
}

//funcion que conecta los dibujos realizados
function dibujarPath(response, lienzo)
{
    if (response.Tipo != 1 && response.Tipo != -1 && response.Tipo != 9)
    {
        for (var i = 0; i < response.Componentes.length - 1; i++)
        {
            var actual = response.Componentes[i];
            dibujarPath(actual, lienzo);
            var siguiente = response.Componentes[i + 1];
            if (response.Tipo != 3 && response.Tipo != 4)
            {
                var tamano_cuadrado = (parseInt(actual.cx) + parseInt(actual.width));
                var tamano_mitad = (parseInt(actual.cy) + parseInt(actual.height / 2) + parseInt(centroLienzo));
                var fin = siguiente.cx;
                lienzo.path("M" + tamano_cuadrado + "," + tamano_mitad + "H" + fin).attr({ 'stroke-width': "2" });
            }            
        }
        if (response.Componentes.length > 0) {
            dibujarPath(response.Componentes[response.Componentes.length - 1], lienzo);
        }
        else
        {
            lienzo.path('M' + response.cx + ',' + (parseInt(response.cy) + parseInt(centroLienzo)) + 'L' + (parseInt(response.cx) + parseInt(response.width)) + ',' + (parseInt(response.cy) + parseInt(centroLienzo) + parseInt(response.height))).attr({ 'stroke-width': "2" });
            lienzo.path('M' + (parseInt(response.cx) + parseInt(response.width)) + ',' + (parseInt(response.cy) + parseInt(centroLienzo)) + 'L' + response.cx + ',' + (parseInt(response.cy) + parseInt(centroLienzo) + parseInt(response.height))).attr({ 'stroke-width': "2" });
        }  
        if (response.Tipo == 2 || response.Tipo == 0)
        {
            if (response.Componentes.length > 0) {
                var primero = response.Componentes[0];
                var ultimo = response.Componentes[response.Componentes.length - 1];
                var tamano_mitad = (parseInt(primero.cy) + parseInt(primero.height / 2) + parseInt(centroLienzo));
                //path que conecta el primero elemento del grupo con las lineas discontinuas del grupo
                lienzo.path("M" + primero.cx + "," + tamano_mitad + " H" + response.cx).attr({ 'stroke-width': "2" });
                //path que conecta el ultimo del grupo con las lineas discontinuas del grupo
                lienzo.path("M" + (parseInt(ultimo.cx) + parseInt(ultimo.width)) + "," + tamano_mitad + " H" + (parseInt(response.width) + parseInt(response.cx))).attr({ 'stroke-width': "2" });
            }
        }
        if (response.Tipo == 3)
        {
            inicio_Barra(response,lienzo);
            final_Barra(response,lienzo);
        }
        if (response.Tipo == 4)
        {
            inicio_Barra(response, lienzo);
            final_Barra(response, lienzo);
        }
    }
}

//Funcion que dibuja las repeticiones para aquellos que las tengan
function dibujar_Path_Repeticiones(response,lienzo)
{
    for (var i = 0; i < response.Componentes.length; i++)
    {
        var actual = response.Componentes[i];
        if (actual.Repeticiones != "" && actual.Repeticiones != null)
        {
            var tamano_mitad = (parseInt(actual.cy) + parseInt(actual.height / 2) + parseInt(centroLienzo));
            if (actual.Tipo == 1 || actual.Tipo == 2 || actual.Tipo == 4)
            {
                if (actual.Repeticiones == "?")
                {
                    lienzo.path("M" + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + "," + tamano_mitad + " V" + (parseInt(tamano_mitad) - parseInt(actual.height / 2) - parseInt(15))).attr({ 'stroke-width': "2" });
                    var inicio = (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8));
                    var final = (parseInt(actual.width + parseInt(16)));
                    lienzo.path('M' + inicio + ',' + (parseInt(tamano_mitad) - parseInt(actual.height / 2) - parseInt(15)) + 'H' + (parseInt(inicio) - parseInt(final))).attr({ 'stroke-width': "2" });
                    lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + (tamano_mitad) + 'V' + (parseInt(tamano_mitad) - parseInt(actual.height / 2) - parseInt(15))).attr({ 'stroke-width': "2" });
                    lienzo.path('M' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + ',' + tamano_mitad + 'L' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8) + parseInt(5)) + ',' + (parseInt(tamano_mitad) - parseInt(8))).attr({ 'stroke-width': "2" });
                    lienzo.path('M' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + ',' + tamano_mitad + 'L' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8) - parseInt(5)) + ',' + (parseInt(tamano_mitad) - parseInt(8))).attr({ 'stroke-width': "2" });
                }
                else
                {
                    if (actual.Repeticiones == "+")
                    {
                        lienzo.path("M" + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + "," + tamano_mitad + " V" + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10))).attr({ 'stroke-width': "2" });
                        var inicio = (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8));
                        var final = (parseInt(actual.width + parseInt(16)));
                        lienzo.path('M' + inicio + ',' + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10)) + 'H' + (parseInt(inicio) - parseInt(final))).attr({ 'stroke-width': "2" });
                        lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10)) + 'V' + tamano_mitad).attr({ 'stroke-width': "2" });
                        lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + tamano_mitad + 'L' + (parseInt(inicio) - parseInt(final) - parseInt(5)) + ',' + (parseInt(tamano_mitad) + parseInt(8))).attr({ 'stroke-width': "2" });
                        lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + tamano_mitad + 'L' + (parseInt(inicio) - parseInt(final) + parseInt(5)) + ',' + (parseInt(tamano_mitad) + parseInt(8))).attr({ 'stroke-width': "2" });
                    }
                    else if (actual.Repeticiones == "*")
                    {
                        lienzo.path("M" + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + "," + tamano_mitad + " V" + (parseInt(tamano_mitad) - parseInt(actual.height / 2) - parseInt(15))).attr({ 'stroke-width': "2" });
                        var inicio = (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8));
                        var final = (parseInt(actual.width + parseInt(16)));
                        lienzo.path('M' + inicio + ',' + (parseInt(tamano_mitad) - parseInt(actual.height / 2) - parseInt(15)) + 'H' + (parseInt(inicio) - parseInt(final))).attr({ 'stroke-width': "2" });
                        lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + (tamano_mitad) + 'V' + (parseInt(tamano_mitad) - parseInt(actual.height / 2) - parseInt(15))).attr({ 'stroke-width': "2" });
                        lienzo.path("M" + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + "," + tamano_mitad + " V" + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10))).attr({ 'stroke-width': "2" });
                        lienzo.path('M' + inicio + ',' + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10)) + 'H' + (parseInt(inicio) - parseInt(final))).attr({ 'stroke-width': "2" });
                        lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10)) + 'V' + tamano_mitad).attr({ 'stroke-width': "2" });
                        lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + tamano_mitad + 'L' + (parseInt(inicio) - parseInt(final) - parseInt(5)) + ',' + (parseInt(tamano_mitad) + parseInt(8))).attr({ 'stroke-width': "2" });
                        lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + tamano_mitad + 'L' + (parseInt(inicio) - parseInt(final) + parseInt(5)) + ',' + (parseInt(tamano_mitad) + parseInt(8))).attr({ 'stroke-width': "2" });
                        lienzo.path('M' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + ',' + tamano_mitad + 'L' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8) + parseInt(5)) + ',' + (parseInt(tamano_mitad) - parseInt(8))).attr({ 'stroke-width': "2" });
                        lienzo.path('M' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + ',' + tamano_mitad + 'L' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8) - parseInt(5)) + ',' + (parseInt(tamano_mitad) - parseInt(8))).attr({ 'stroke-width': "2" });
                    }
                    else
                    {
                        /*Las repeticiones vienen desde el server con una marca la letra b significa que todo esta correcto*/
                        if (actual.Repeticiones.substr(0, 1) == 'b')
                        {
                            actual.Repeticiones = actual.Repeticiones.substr(1);
                            lienzo.path("M" + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + "," + tamano_mitad + " V" + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10))).attr({ 'stroke-width': "2" });
                            var inicio = (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8));
                            var final = (parseInt(actual.width + parseInt(16)));
                            lienzo.path('M' + inicio + ',' + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10)) + 'H' + (parseInt(inicio) - parseInt(final))).attr({ 'stroke-width': "2" });
                            lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10)) + 'V' + tamano_mitad).attr({ 'stroke-width': "2" });
                            lienzo.text((parseInt(actual.cx) + parseInt(actual.width) - parseInt(13)), (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(17)), actual.Repeticiones + ' ' + comprobar('Repeticiones')).attr({ 'font-size': 11, 'font-family': "Times" });
                            lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + tamano_mitad + 'L' + (parseInt(inicio) - parseInt(final) - parseInt(5)) + ',' + (parseInt(tamano_mitad) + parseInt(8))).attr({ 'stroke-width': "2" });
                            lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + tamano_mitad + 'L' + (parseInt(inicio) - parseInt(final) + parseInt(5)) + ',' + (parseInt(tamano_mitad) + parseInt(8))).attr({ 'stroke-width': "2" });
                        }
                        else {
                            if (actual.Repeticiones.substr(0, 1) == 'o') {
                                actual.Repeticiones = actual.Repeticiones.substr(1);
                                lienzo.path("M" + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + "," + tamano_mitad + " V" + (parseInt(tamano_mitad) - parseInt(actual.height / 2) - parseInt(15))).attr({ 'stroke-width': "2" });
                                var inicio = (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8));
                                var final = (parseInt(actual.width + parseInt(16)));
                                lienzo.path('M' + inicio + ',' + (parseInt(tamano_mitad) - parseInt(actual.height / 2) - parseInt(15)) + 'H' + (parseInt(inicio) - parseInt(final))).attr({ 'stroke-width': "2" });
                                lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + (tamano_mitad) + 'V' + (parseInt(tamano_mitad) - parseInt(actual.height / 2) - parseInt(15))).attr({ 'stroke-width': "2" });
                                lienzo.path("M" + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + "," + tamano_mitad + " V" + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10))).attr({ 'stroke-width': "2" });
                                lienzo.path('M' + inicio + ',' + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10)) + 'H' + (parseInt(inicio) - parseInt(final))).attr({ 'stroke-width': "2" });
                                lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10)) + 'V' + tamano_mitad).attr({ 'stroke-width': "2" });
                                lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + tamano_mitad + 'L' + (parseInt(inicio) - parseInt(final) - parseInt(5)) + ',' + (parseInt(tamano_mitad) + parseInt(8))).attr({ 'stroke-width': "2" });
                                lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + tamano_mitad + 'L' + (parseInt(inicio) - parseInt(final) + parseInt(5)) + ',' + (parseInt(tamano_mitad) + parseInt(8))).attr({ 'stroke-width': "2" });
                                lienzo.path('M' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + ',' + tamano_mitad + 'L' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8) + parseInt(5)) + ',' + (parseInt(tamano_mitad) - parseInt(8))).attr({ 'stroke-width': "2" });
                                lienzo.path('M' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + ',' + tamano_mitad + 'L' + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8) - parseInt(5)) + ',' + (parseInt(tamano_mitad) - parseInt(8))).attr({ 'stroke-width': "2" });
                                lienzo.text((parseInt(actual.cx) + parseInt(actual.width) - parseInt(13)), (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(17)), actual.Repeticiones + ' ' + comprobar('Repeticiones')).attr({ 'font-size': 11, 'font-family': "Times" });
                            }
                            else
                            {
                                wrong = true;
                                actual.Repeticiones = actual.Repeticiones.substr(1);
                                var linea3 = lienzo.path("M" + (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8)) + "," + tamano_mitad + " V" + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10))).attr({ 'stroke-width': '2' });
                                var inicio = (parseInt(actual.cx) + parseInt(actual.width) + parseInt(8));
                                var final = (parseInt(actual.width + parseInt(16)));
                                var linea4 = lienzo.path('M' + inicio + ',' + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10)) + 'H' + (parseInt(inicio) - parseInt(final))).attr({ 'stroke-width': "2" });
                                var linea5 = lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(10)) + 'V' + tamano_mitad).attr({ 'stroke-width': "2" });
                                lienzo.text((parseInt(actual.cx) + parseInt(actual.width) - parseInt(13)), (parseInt(actual.height / 2) + parseInt(tamano_mitad) + parseInt(17)), actual.Repeticiones + ' ' + comprobar('Repeticiones')).attr({ 'font-size': 11, 'font-family': "Times" });
                                var linea1 = lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + tamano_mitad + 'L' + (parseInt(inicio) - parseInt(final) - parseInt(5)) + ',' + (parseInt(tamano_mitad) + parseInt(8))).attr({ 'stroke-width': "2" });
                                var linea2 = lienzo.path('M' + (parseInt(inicio) - parseInt(final)) + ',' + tamano_mitad + 'L' + (parseInt(inicio) - parseInt(final) + parseInt(5)) + ',' + (parseInt(tamano_mitad) + parseInt(8))).attr({ 'stroke-width': "2" });
                                function rojo() {
                                    linea1.animate({ stroke: '#9B2727' }, 600);
                                    linea2.animate({ stroke: '#9B2727' }, 600);
                                    linea3.animate({ stroke: '#9B2727' }, 600);
                                    linea4.animate({ stroke: '#9B2727' }, 600);
                                    linea5.animate({ stroke: '#9B2727' }, 600, blanco);
                                }
                                function blanco() {
                                    linea1.animate({ r: 6, stroke: '#dae1e1' }, 600);
                                    linea2.animate({ r: 6, stroke: '#dae1e1' }, 600);
                                    linea3.animate({ r: 6, stroke: '#dae1e1' }, 600);
                                    linea4.animate({ r: 6, stroke: '#dae1e1' }, 600);
                                    linea5.animate({ r: 6, stroke: '#dae1e1' }, 600, rojo);
                                }
                                rojo();
                            }
                        }
                    }
                }
            }
        }
        if (actual.Tipo != 1 && actual.Tipo != -1 && actual.Tipo != 9)
        {
            dibujar_Path_Repeticiones(actual, lienzo);
        }        
    }
}

//Funcion que dibuja el inicio del camino de la |
function inicio_Barra(response, lienzo)
{
    var primero = response.Componentes[0];
    var ultimo = response.Componentes[response.Componentes.length - 1];
    var tamano_mitad = (parseInt(primero.cy) + parseInt(primero.height / 2) + parseInt(centroLienzo));
    var fin = (parseInt(ultimo.cy) + parseInt(ultimo.height / 2) + parseInt(centroLienzo));
    for (var i = 0; i < response.Componentes.length; i++) {
        var actual = response.Componentes[i];
        lienzo.path('M' + actual.cx + ',' + (parseInt(actual.cy) + parseInt(actual.height / 2) + parseInt(centroLienzo)) + 'H' + response.cx).attr({ 'stroke-width': "2" });
    }
    lienzo.path("M" + response.cx + "," + tamano_mitad + " V" + fin).attr({ 'stroke-width': "2" });
}

//Funcion que dibuja el final del camino de la |
function final_Barra(response,lienzo)
{
    var primero = response.Componentes[0];
    var ultimo = response.Componentes[response.Componentes.length - 1];
    var tamano_mitad = (parseInt(primero.cy) + parseInt(primero.height / 2) + parseInt(centroLienzo));
    var fin = (parseInt(ultimo.cy) + parseInt(ultimo.height / 2) + parseInt(centroLienzo));
    for(var i = 0; i < response.Componentes.length; i++)
    {
        var actual = response.Componentes[i];
        lienzo.path('M' + (parseInt(actual.cx) + parseInt(actual.width)) + ',' + (parseInt(actual.cy) + parseInt(actual.height / 2) + parseInt(centroLienzo)) + 'H' + (parseInt(response.cx) + parseInt(response.width))).attr({ 'stroke-width': "2" });
    }
    lienzo.path("M" + (parseInt(response.cx) + parseInt(response.width)) + "," + tamano_mitad + " V" + fin).attr({ 'stroke-width': "2" });
}

//Funcion que se llama en Visualizar
function success(response) {
    if (response.Componentes.length == 0 && response.Valor == null) {
        alert(comprobar("Expresión Vacía"));
    }
    else {
        var contenedor = $("#result")[0];
        calcularCoordenadas(response, cx, cy);
        var lienzo = Raphael(contenedor, lienzoEjeX, lienzoEjeY);
        centroLienzo = lienzo.height / 2;
        dibujar(response, lienzo);
        dibujarPath(response, lienzo);
        dibujar_Path_Repeticiones(response, lienzo);
        dibujar_inicio_fin(response, lienzo);
        addInput();
        desp = 0;
    }
}

//Funcion que se llama en Ocurrencia
function successMatch(response)
{
    para_match(response);
}

//Funcion para doblar las barras para llevarlas a Java
function doble_barra(texto)
{
    if (texto.indexOf("\\") > -1)
    {        
        $("#same").append(comprobar("use") + $('#txtData').val().replace(/\\/g, "\\\\"));
        $("#same").append(saltoLinea);
    }
}

//Funcion que va creado una tabla dinamica con los elementos devueltos del servidor
function para_match(response)
{
    if (response.length == 0) {
        $('#expresion').append('<div id="close">&nbsp;&nbsp;X</div>');
        $("#expresion").append(comprobar("resultadoMacht"));
        $("#expresion").append(saltoLinea + saltoLinea);   
    }
    else {
        var Input_Ocurrencia_Tratada = $('#secondInput').val().replace(/</gi, '&lt;');
        alert(Input_Ocurrencia_Tratada);
        $('#expresion').append('<div id="close">&nbsp;&nbsp;X</div>');        
        var tabla;
        $("#expresion").append(saltoLinea);
        crear_Tabla();
        $("#expresion").append(saltoLinea);
        for (var i = 0; i < response.length; i++) {
            var contador_grupos = 0;
            var palabraRojo = $('#secondInput').val().substr(0, response[i].Ind) + "<font color=#FF000D>" + $('#secondInput').val().substr(response[i].Ind, response[i].Len) + "</font>" + $('#secondInput').val().substr(response[i].Len + response[i].Ind);
            $("#nueva").append('<tr><td>' + (parseInt(i) + parseInt(1)) + '</td><td><table id = "nueva"><tr><td>' + palabraRojo + '</td></tr><tr id="blue' + i + '"></tr></table></td></tr>');
            var ocurrencia = $('#secondInput').val().substr(response[i].Ind, response[i].Len);
            for (var j = 0; j < response[i].Grupos.length; j++) {
                var inicio = response[i].Grupos[j] - response[i].Ind;
                var fin = response[i].Grupos[++j];
                if (fin > 0) {
                    var palabraAzul = ocurrencia.substr(0, inicio) + "<font color=blue>" + ocurrencia.substr(inicio, fin) + "</font>" + ocurrencia.substr(inicio + fin) + saltoLinea;
                    tabla = '<table><tr><td>' + comprobar("Grupo") + contador_grupos + '</td><td>' + palabraAzul + '</td></tr></table>';
                    $('#blue' + i).append(tabla);
                    contador_grupos = contador_grupos + 1;
                }
            }
        }
        $("#expresion").append(saltoLinea);
    }
    $("#expresion").css('position', 'relative');
}

//funcion para crear una tabla
function crear_Tabla()
{
    var tabla = '<table id = "nueva"><tr><td>' + comprobar("match") + '</td><td>'+ comprobar("valor")+'</td></tr></table>'
    $("#expresion").append(tabla);
}

//funcion que mostrara en caso de error
function error(response) {
    alert(comprobar("server"));
}