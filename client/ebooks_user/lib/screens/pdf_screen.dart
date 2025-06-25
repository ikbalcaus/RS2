import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:syncfusion_flutter_pdfviewer/pdfviewer.dart";

class PdfScreen extends StatefulWidget {
  final String? filePath;
  const PdfScreen({super.key, this.filePath});

  @override
  State<PdfScreen> createState() => _PdfScreenState();
}

class _PdfScreenState extends State<PdfScreen> {
  final bool _isLoading = false;

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      showBackButton: true,
      child: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _buildResultView(),
    );
  }

  Widget _buildResultView() {
    return SfPdfViewer.network(
      "${Globals.apiAddress}/pdfs/summary/${widget.filePath}.pdf",
      onDocumentLoadFailed: (details) {
        WidgetsBinding.instance.addPostFrameCallback((_) {
          if (!mounted) return;
          Helpers.showErrorMessage(context, "PDF not found");
          Navigator.pop(context);
        });
      },
    );
  }
}
