# Frontdesk Console Application - User Guide

## Getting Started

### Prerequisites
- .NET 10 SDK installed
- SQL Server with LMS database configured
- Connection string in `appsettings.json`

### Installation

1. The Frontdesk console app is already integrated with the LMS solution
2. Ensure all required students and books are created via Librarian app first
3. Ensure book copies are added via Librarian app

### Running the Application

```bash
cd LMS.Frontdesk.ConsoleApp
dotnet run
```

## Main Menu Options

### Option 1: Issue Book to Student

**Purpose**: Record when a book is issued to a student

**Steps**:
1. Select option `1` from main menu
2. Enter the Student ID (numeric)
3. System displays student name for confirmation
4. System shows all available books with:
   - Book ID
   - Book Name
   - Authors
   - Number of issued copies
   - Number of available copies
5. Enter the Book ID you want to issue
6. System confirms the issue and provides:
   - Issue ID (for future returns)
   - Updated available copy count

**Example Flow**:
```
Select option: 1
Student ID: 5
Student: John Doe

Books in library:
1: The Great Gatsby | Authors: F. Scott Fitzgerald | Issued: 2 | Available: 3
2: 1984 | Authors: George Orwell | Issued: 1 | Available: 2
3: To Kill a Mockingbird | Authors: Harper Lee | Issued: 0 | Available: 5

Book ID to issue: 1
Book issued successfully. Issue ID: 10
Book: The Great Gatsby | Available copies now: 2
```

**Validation**:
- ✓ Student must exist in system
- ✓ Book must exist in system
- ✓ Must have at least one available copy
- ✗ Will show error if any validation fails

---

### Option 2: Return Book from Student

**Purpose**: Record when a book is returned by a student

**Steps**:
1. Select option `2` from main menu
2. Enter the Issue ID (from when the book was issued)
3. System confirms the return and provides:
   - Issue ID
   - Student ID
   - Copy ID
   - Confirmation message

**Example Flow**:
```
Select option: 2
Issue ID: 10
Book returned successfully.
Issue ID: 10 | Student ID: 5 | Copy ID: 3
```

**Validation**:
- ✓ Issue ID must exist
- ✓ Book must not already be returned (ReturnDate must be NULL)
- ✗ Will show error if issue already returned or not found

---

### Option 3: View All Books

**Purpose**: Display current inventory status

**Output Format**:
```
Books in library:
{BookId}: {BookName} | Authors: {Author1, Author2, ...} | Issued: {Count} | Available: {Count}
```

**Example Output**:
```
Books in library:
1: The Great Gatsby | Authors: F. Scott Fitzgerald | Issued: 2 | Available: 3
2: 1984 | Authors: George Orwell | Issued: 1 | Available: 2
3: To Kill a Mockingbird | Authors: Harper Lee | Issued: 0 | Available: 5
4: The Catcher in the Rye | Authors: J.D. Salinger | Issued: 4 | Available: 1
```

**Useful For**:
- Checking copy availability before issuing
- Tracking library inventory
- Finding book IDs needed for issues and returns

---

### Option 4: View Issued Books

**Purpose**: See all currently active book issues

**Output Format**:
```
Currently Issued Books:
Issue ID: {IssueId} | Student ID: {StudentId} | Copy ID: {CopyId} | Issue Date: {Date}
```

**Example Output**:
```
Currently Issued Books:
Issue ID: 10 | Student ID: 5 | Copy ID: 3 | Issue Date: 2024-01-15
Issue ID: 11 | Student ID: 7 | Copy ID: 1 | Issue Date: 2024-01-15
Issue ID: 12 | Student ID: 3 | Copy ID: 5 | Issue Date: 2024-01-15
```

**Useful For**:
- Finding Issue IDs for returns
- Tracking active issues
- Monitoring book distribution

---

### Option 0: Exit

Closes the application

---

## Common Workflows

### Workflow 1: Complete Issue and Return Cycle

```
1. User selects Option 3 (View Books)
   → Identifies Book ID and availability

2. User selects Option 1 (Issue Book)
   → Enters Student ID and Book ID
   → Note the Issue ID: 10

3. [Some time passes...]

4. User selects Option 2 (Return Book)
   → Enters Issue ID: 10
   → Book is returned

5. User selects Option 4 (View Issued Books)
   → Confirms Issue ID 10 is no longer listed
```

### Workflow 2: Check Current Inventory

```
1. User selects Option 3 (View Books)
2. User selects Option 4 (View Issued Books)
3. User can calculate real-time availability
```

---

## Error Messages and Solutions

| Error Message | Cause | Solution |
|---|---|---|
| Invalid student ID | Entered non-numeric value | Enter numeric Student ID |
| Student not found | Student ID doesn't exist | Check Student ID with Librarian or Admin |
| Invalid book ID | Entered non-numeric value | Enter numeric Book ID |
| Book not found | Book ID doesn't exist | Use Option 3 to find valid Book IDs |
| No available copies | All copies are issued | Wait for returns or get more copies from Librarian |
| Invalid issue ID | Issue ID doesn't exist | Use Option 4 to find valid Issue IDs |
| Issue not found or book already returned | Issue was already returned | Select Option 4 to view active issues |

---

## Tips and Best Practices

1. **Before Issuing a Book**:
   - Use Option 3 to verify the book exists and has available copies
   - Write down the Issue ID after issuing for future reference

2. **Before Returning a Book**:
   - Use Option 4 to find the correct Issue ID if you don't remember it
   - Ensure you're entering the right Issue ID

3. **Monitoring**:
   - Use Option 4 regularly to track active issues
   - Use Option 3 to identify books running low on copies

4. **Data Accuracy**:
   - All Student IDs and Book IDs should be set up via Librarian app first
   - Book copies should be added via Librarian app

---

## Technical Details

### Copy Count Management
- **Issued Count**: Number of copies currently issued (ReturnDate = NULL)
- **Available Count**: Number of copies that can be issued (Total - Issued)
- **Total Count**: Sum of all copies created in system

### Issue ID Format
- Issue IDs are auto-generated integers
- Unique identifier for each book-student transaction
- Required for book returns

### Dates
- Issue Date: Automatically set to current system date/time
- Return Date: Automatically set when book is returned

---

## Support

For issues or questions:
1. Check error messages for guidance
2. Verify data setup via Librarian app
3. Ensure SQL Server connection in appsettings.json
4. Contact system administrator if database issues persist
