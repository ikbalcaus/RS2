import "dart:io";
import "package:ebooks_user/models/access_rights/access_right.dart";
import "package:ebooks_user/providers/access_rights_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:syncfusion_flutter_pdfviewer/pdfviewer.dart";

class PdfScreen extends StatefulWidget {
  final int? bookId;
  final String filePath;

  const PdfScreen({super.key, this.bookId, required this.filePath});

  @override
  State<PdfScreen> createState() => _PdfScreenState();
}

class _PdfScreenState extends State<PdfScreen> {
  late AccessRightsProvider _accessRightsProvider;
  AccessRight? _accessRight;
  File? _localFile;
  bool _isLoading = true;
  final PdfViewerController _pdfController = PdfViewerController();

  @override
  void initState() {
    super.initState();
    _accessRightsProvider = context.read<AccessRightsProvider>();
    if (widget.bookId != null) {
      _fetchAccessRight();
      _loadLocalBookFile();
    }
  }

  @override
  void dispose() {
    _pdfController.dispose();
    super.dispose();
  }

  Future _fetchAccessRight() async {
    try {
      final accessRight = await _accessRightsProvider.getById(widget.bookId!);
      if (!mounted) return;
      setState(() => _accessRight = accessRight);
    } catch (ex) {
      if (!mounted) return;
      setState(() => _accessRight = null);
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
    }
  }

  Future _saveLastReadPage() async {
    try {
      await _accessRightsProvider.saveLastReadPage(
        widget.bookId!,
        _pdfController.pageNumber,
      );
      Helpers.showErrorMessage(context, "Successfully saved reading progress");
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      showBackButton: true,
      onBookmarkClicked: _accessRight != null
          ? () async => await _saveLastReadPage()
          : null,
      child: widget.bookId != null ? _bookView() : _bookSummaryView(),
    );
  }

  Future _loadLocalBookFile() async {
    try {
      final file = File(widget.filePath);
      if (await file.exists()) {
        setState(() {
          _localFile = file;
          _isLoading = false;
        });
      }
    } catch (ex) {
      if (!mounted) return;
      Helpers.showErrorMessage(context, ex);
      Navigator.pop(context);
    }
  }

  Widget _bookView() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }
    return SfPdfViewer.file(
      _localFile!,
      controller: _pdfController,
      onDocumentLoaded: (details) {
        if ((_accessRight?.lastReadPage ?? 0) > 0 &&
            (_accessRight?.lastReadPage ?? 0) <= details.document.pages.count) {
          _pdfController.jumpToPage((_accessRight?.lastReadPage ?? 0));
        }
      },
    );
  }

  Widget _bookSummaryView() {
    return Stack(
      children: [
        SfPdfViewer.network(
          "${Globals.apiAddress}/pdfs/summary/${widget.filePath}.pdf",
          onDocumentLoaded: (details) {
            setState(() {
              _isLoading = false;
            });
          },
          onDocumentLoadFailed: (details) {
            WidgetsBinding.instance.addPostFrameCallback((_) {
              if (!mounted) return;
              Helpers.showErrorMessage(context, "PDF not found");
              Navigator.pop(context);
            });
          },
        ),
        if (_isLoading) const Center(child: CircularProgressIndicator()),
      ],
    );
  }
}
