export function openPrintWindow(labelsHtml, orderNumber) {
    const printWindow = window.open('', '_blank');

    if (printWindow) {
        printWindow.document.write(`
            <html>
                <head>
                    <title>Print Labels</title>
                    <style>
                        @page {
                            size: 4in 2in; /* Set the label size to 4 inches wide and 2 inches tall */
                            margin: 0;
                        }
                        body {
                            font-family: Arial, sans-serif;
                            text-align: center;
                            margin: 0;
                            padding: 0;
                        }
                        .label {
                            width: 4in;
                            height: 2in;
                            border: 1px solid black;
                            display: flex;
                            flex-direction: column;
                            justify-content: center;
                            align-items: center;
                            padding: 5px;
                            box-sizing: border-box;
                            page-break-after: always;
                        }
                        .label h4 {
                            margin: 3px 0;
                            font-size: 14px;
                        }
                        .barcode {
                            margin-top: 5px;
                            height: 1in;
                        }
                    </style>
                </head>
                <body>
                    ${labelsHtml}
                    <script src="https://cdn.jsdelivr.net/npm/jsbarcode@3.11.5/dist/JsBarcode.all.min.js"></script>
                    <script>
                        function generateBarcodes() {
                            const barcodeElements = document.querySelectorAll('.barcode');
                            barcodeElements.forEach((element) => {
                                const barcodeValue = element.dataset.barcodeValue;
                                if (barcodeValue) {
                                    JsBarcode(element, barcodeValue, {
                                        format: "CODE128",
                                        width: 1.5,
                                        height: 40,
                                        displayValue: true,
                                        fontSize: 12
                                    });
                                } else {
                                    console.error('No barcode value found for element:', element);
                                }
                            });
                        }
                        window.onload = function() {
                            generateBarcodes();
                            window.print();
                            setTimeout(() => window.close(), 1000);
                        };
                    </script>
                </body>
            </html>
        `);

        printWindow.document.close();
    } else {
        alert('Failed to open print window. Please check your browser settings.');
    }
}
export function triggerLabelPrinting(orderNumber) {
    console.log("Printing labels for Order:", orderNumber);

    const labels = [];

    $(".product-row").each(function () {
        const $row = $(this);
        const productName = $row.find("input[readonly]").val();
        const productBarcodeID = $row.find(".product-barcode").data("barcode-value");

        if (productName && productBarcodeID) {
            labels.push(`
                <div class="label">
                    <h4>Orden: ${orderNumber}</h4>
                    <h4>Producto: ${productName}</h4>
                    <svg class="barcode" data-barcode-value="${productBarcodeID}"></svg>
                </div>
            `);
        }
    });

    if (labels.length > 0) {
        openPrintWindow(labels.join(''), orderNumber);
    } else {
        console.warn("No hay datos para imprimir etiquetas.");
    }
}