export function openPrintWindow(labels) {
    const printWindow = window.open("", "PRINT", "width=600,height=800");

    // Write HTML structure
    printWindow.document.write(`
        <html>
            <head>
                <title>Imprimir Etiquetas</title>
                <style>
                    body { font-family: Arial, sans-serif; margin: 0; padding: 0; }
                    h4 { font-size: 16px; }
                    p { font-size: 14px; margin: 5px 0; }
                    .label { page-break-after: always; }
                </style>
            </head>
            <body>
                ${labels}
                <script>
                    const script = document.createElement("script");
                    script.src = "https://cdn.jsdelivr.net/npm/jsbarcode@3.11.5/dist/JsBarcode.all.min.js";
                    script.onload = function () {
                        console.log("JsBarcode loaded successfully.");
                        const productRows = document.querySelectorAll("[id^='barcode-']");
                        productRows.forEach((element, index) => {
                            const productName = document.querySelectorAll("h4")[index].textContent.replace("Producto: ", "");
                            JsBarcode("#" + element.id, productName, {
                                format: "CODE128",
                                width: 2,
                                height: 50,
                                displayValue: true
                            });
                        });
                        setTimeout(() => {
                            window.print();
                            window.close();
                        }, 500);
                    };
                    script.onerror = function () {
                        console.error("Failed to load JsBarcode.");
                    };
                    document.head.appendChild(script);
                </script>
            </body>
        </html>
    `);

    printWindow.document.close();
}
