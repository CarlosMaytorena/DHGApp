import { openPrintWindow } from "/js/labelPrinter.js";

$(document).ready(function () {
    // Add Product Row
    $("#addProductRow").click(function () {
        let newRow = $(".product-row:first").clone();
        newRow.find("input, select").val(""); // Clear values in the new row
        $("#productContainer").append(newRow);
    });

    // Remove Product Row
    $("#removeProductRow").click(function () {
        if ($(".product-row").length > 1) {
            $(".product-row:last").remove();
        }
    });

    // Print Labels Handler
    $("#imprimirEtiquetas").click(function () {
        let orderNumber = $("input[placeholder='Ingrese el número de orden de compra']").val();

        // Gather product data
        let products = [];
        $(".product-row").each(function () {
            products.push({
                productName: $(this).find("select option:selected").text(),
                cantidadRecibida: $(this).find("input[placeholder='Recibida']").val(),
                unidad: $(this).find("input[placeholder='Unidad']").val(),
            });
        });

        // Generate labels
        const labels = products.map((product, index) => {
            if (product.productName && product.cantidadRecibida && product.unidad) {
                let barcodeId = `barcode-${index}`;
                return `
                <div class="label" style="border: 1px solid #000; margin: 10px; padding: 10px; width: 300px; page-break-after: always;">
                    <h4 style="margin: 0;">Producto: ${product.productName}</h4>
                    <p>Orden: ${orderNumber}</p>
                    <p>Cantidad Recibida: ${product.cantidadRecibida}</p>
                    <p>Unidad: ${product.unidad}</p>
                    <svg id="${barcodeId}" style="margin-top: 10px;"></svg>
                </div>`;
            }
            return "";
        }).join("");

        if (!labels) {
            alert("No hay datos para imprimir etiquetas.");
            return;
        }

        // Call the print window function
        openPrintWindow(labels);
    });
});
