import "package:ebooks_admin/models/overview/overview.dart";
import "package:ebooks_admin/providers/overview_provider.dart";
import "package:ebooks_admin/screens/authors_screen.dart";
import "package:ebooks_admin/screens/books_screen.dart";
import "package:ebooks_admin/screens/genres_screen.dart";
import "package:ebooks_admin/screens/languages_screen.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "package:ebooks_admin/screens/purchases_screen.dart";
import "package:ebooks_admin/screens/questions_screen.dart";
import "package:ebooks_admin/screens/reports_screen.dart";
import "package:ebooks_admin/screens/reviews_screen.dart";
import "package:ebooks_admin/screens/users_screen.dart";
import "package:ebooks_admin/utils/globals.dart";
import "package:ebooks_admin/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";

class OverviewScreen extends StatefulWidget {
  final String? status;
  const OverviewScreen({super.key, this.status});

  @override
  State<OverviewScreen> createState() => _OverviewScreenState();
}

class _OverviewScreenState extends State<OverviewScreen> {
  late OverviewProvider _overviewProvider;
  Overview? _overview;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _overviewProvider = context.read<OverviewProvider>();
    _fetchOverview();
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _buildResultView(),
          ),
        ],
      ),
    );
  }

  Future _fetchOverview() async {
    setState(() => _isLoading = true);
    try {
      final overview = await _overviewProvider.getAllCount();
      if (!mounted) return;
      setState(() => _overview = overview);
    } catch (ex) {
      if (!mounted) return;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (!mounted) return;
        Helpers.showErrorMessage(context, ex);
      });
    } finally {
      if (!mounted) return;
      setState(() => _isLoading = false);
    }
  }

  Widget _buildResultView() {
    final cards = [
      {
        "title": "Books",
        "icon": Icons.book,
        "values": {
          "All": _overview?.booksCount ?? 0,
          "Approved": _overview?.approvedBooksCount ?? 0,
          "Awaited": _overview?.awaitedBooksCount ?? 0,
          "Drafted": _overview?.draftedCount ?? 0,
          "Hidden": _overview?.hiddenCount ?? 0,
          "Rejected": _overview?.rejectedCount ?? 0,
        },
        "onTap": () {
          Navigator.pushReplacement(
            context,
            MaterialPageRoute(builder: (_) => BooksScreen()),
          );
        },
      },
      {
        "title": "Users",
        "icon": Icons.person,
        "values": {"Users": _overview?.usersCount ?? 0},
        "onTap": () => Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (_) => UsersScreen()),
        ),
      },
      {
        "title": "Authors",
        "icon": Icons.person_outline,
        "values": {"Authors": _overview?.authorsCount ?? 0},
        "onTap": () => Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (_) => AuthorsScreen()),
        ),
      },
      {
        "title": "Genres",
        "icon": Icons.category,
        "values": {"Genres": _overview?.genresCount ?? 0},
        "onTap": () => Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (_) => GenresScreen()),
        ),
      },
      {
        "title": "Languages",
        "icon": Icons.language,
        "values": {"Languages": _overview?.languagesCount ?? 0},
        "onTap": () => Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (_) => LanguagesScreen()),
        ),
      },
      {
        "title": "Questions",
        "icon": Icons.question_mark,
        "values": {
          "All": _overview?.questionsCount ?? 0,
          "Answered": _overview?.answeredQuestionsCount ?? 0,
          "Unanswered": _overview?.unansweredQuestionsCount ?? 0,
        },
        "onTap": () => Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (_) => QuestionsScreen()),
        ),
      },
      {
        "title": "Reports",
        "icon": Icons.report,
        "values": {"Reports": _overview?.reportsCount ?? 0},
        "onTap": () => Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (_) => ReportsScreen()),
        ),
      },
      {
        "title": "Reviews",
        "icon": Icons.reviews,
        "values": {"Reviews": _overview?.reviewsCount ?? 0},
        "onTap": () => Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (_) => ReviewsScreen()),
        ),
      },
      {
        "title": "Purchases",
        "icon": Icons.payment,
        "values": {"Purchases": _overview?.purchasesCount ?? 0},
        "onTap": () => Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (_) => PurchasesScreen()),
        ),
      },
    ];

    return Padding(
      padding: const EdgeInsets.all(Globals.spacing),
      child: GridView.builder(
        itemCount: cards.length,
        gridDelegate: SliverGridDelegateWithMaxCrossAxisExtent(
          maxCrossAxisExtent: 450,
          crossAxisSpacing: Globals.spacing,
          mainAxisSpacing: Globals.spacing,
          childAspectRatio: 1,
        ),
        itemBuilder: (context, index) {
          final card = cards[index];
          final values = card["values"] as Map<String, int>;
          return InkWell(
            onTap: card["onTap"] as void Function(),
            borderRadius: BorderRadius.circular(16),
            child: Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  colors: [
                    Globals.backgroundColor.shade100,
                    Globals.backgroundColor.shade200,
                  ],
                  begin: Alignment.topCenter,
                  end: Alignment.bottomCenter,
                ),
                borderRadius: BorderRadius.circular(16),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.center,
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  const SizedBox(height: 20),
                  Icon(card["icon"] as IconData, size: 70, color: Colors.white),
                  Text(
                    card["title"] as String,
                    style: const TextStyle(
                      fontSize: 20,
                      fontWeight: FontWeight.bold,
                      color: Colors.white,
                    ),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 4),
                  ...values.entries.map(
                    (e) => Text(
                      "${e.key}: ${e.value}",
                      style: const TextStyle(
                        fontSize: 16,
                        color: Colors.white70,
                      ),
                      textAlign: TextAlign.center,
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}
