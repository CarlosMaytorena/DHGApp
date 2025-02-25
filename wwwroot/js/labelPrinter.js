export function openPrintWindow(labelsHtml, orderNumber) {
    const printWindow = window.open('', '_blank');
    if (printWindow) {
        printWindow.document.write(`
            <html>
                <head>
                    <title>Print Labels</title>
                    <style>
                        .label {
                            border: 1px solid black;
                            margin: 10px;
                            padding: 10px;
                            width: 300px;
                            text-align: center;
                            page-break-after: always;
                        }
                        .barcode {
                            margin-top: 10px;
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
                                const barcodeValue = element.dataset.barcodeValue; // Use ProductBarcodeID
                                if (barcodeValue) {
                                    JsBarcode(element, barcodeValue, {
                                        format: "CODE128",
                                        width: 2,
                                        height: 50,
                                        displayValue: true,
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
