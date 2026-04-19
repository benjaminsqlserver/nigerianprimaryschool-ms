namespace NigerianPrimarySchool.Domain.Constants;

/// <summary>
/// Defines all application roles for a Nigerian Primary School.
/// This is the single source of truth — referenced by Infrastructure seeding,
/// Authorization policies, and UI navigation guards.
/// </summary>
public static class Roles
{
    // ── Administrative ──────────────────────────────────────────────────────
    /// <summary>Full unrestricted access to the entire application.</summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>School principal / head teacher. Access to academic & admin data.</summary>
    public const string HeadTeacher = "HeadTeacher";

    // ── Teaching Staff ───────────────────────────────────────────────────────
    /// <summary>Teacher assigned to specific subjects across classes.</summary>
    public const string SubjectTeacher = "SubjectTeacher";

    /// <summary>Teacher responsible for a particular class/arm.</summary>
    public const string ClassTeacher = "ClassTeacher";

    // ── Finance / Store Staff ─────────────────────────────────────────────
    /// <summary>Manages school fees, levies and financial records.</summary>
    public const string SchoolAccountant = "SchoolAccountant";

    /// <summary>Alias role — same permissions as SchoolAccountant for bursary ops.</summary>
    public const string SchoolBursar = "SchoolBursar";

    /// <summary>Manages the school bookshop — stock and sales.</summary>
    public const string SchoolBookShopKeeper = "SchoolBookShopKeeper";

    /// <summary>Manages general school store — supplies, equipment, stationery.</summary>
    public const string SchoolStoreKeeper = "SchoolStoreKeeper";

    // ── Community ────────────────────────────────────────────────────────────
    /// <summary>Parent or legal guardian of one or more enrolled students.</summary>
    public const string Parent = "Parent";

    /// <summary>Enrolled pupil. Read-only access to own records.</summary>
    public const string Student = "Student";

    // ── Convenience arrays (useful for policy builders & UI) ─────────────────

    /// <summary>All roles that are considered staff (non-community).</summary>
    public static readonly IReadOnlyList<string> AllStaff =
    [
        SuperAdmin, HeadTeacher, SubjectTeacher, ClassTeacher,
        SchoolAccountant, SchoolBursar, SchoolBookShopKeeper, SchoolStoreKeeper
    ];

    /// <summary>Roles with finance/fee access.</summary>
    public static readonly IReadOnlyList<string> FinanceRoles =
        [SchoolAccountant, SchoolBursar, SuperAdmin, HeadTeacher];

    /// <summary>All teaching roles.</summary>
    public static readonly IReadOnlyList<string> TeachingRoles =
        [HeadTeacher, SubjectTeacher, ClassTeacher];

    /// <summary>All store/inventory roles.</summary>
    public static readonly IReadOnlyList<string> StoreRoles =
        [SchoolBookShopKeeper, SchoolStoreKeeper, SuperAdmin];

    /// <summary>Community roles (non-staff).</summary>
    public static readonly IReadOnlyList<string> CommunityRoles =
        [Parent, Student];

    /// <summary>All roles in the system — used for seeding.</summary>
    public static readonly IReadOnlyList<string> All =
    [
        SuperAdmin, HeadTeacher, SubjectTeacher, ClassTeacher,
        SchoolAccountant, SchoolBursar, SchoolBookShopKeeper,
        SchoolStoreKeeper, Parent, Student
    ];
}

/// <summary>
/// Named authorization policy keys — map directly to Role groupings above.
/// Register these in Program.cs via AddAuthorization().
/// </summary>
public static class Policies
{
    public const string RequireSuperAdmin       = "RequireSuperAdmin";
    public const string RequireHeadTeacherOrAbove = "RequireHeadTeacherOrAbove";
    public const string RequireAnyStaff         = "RequireAnyStaff";
    public const string RequireTeacher          = "RequireTeacher";
    public const string RequireFinance          = "RequireFinance";
    public const string RequireStore            = "RequireStore";
    public const string RequireParentOrStudent  = "RequireParentOrStudent";
    public const string RequireParent           = "RequireParent";
    public const string RequireStudent          = "RequireStudent";
}
