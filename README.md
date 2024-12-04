
# **Technical Aptitude Assessment**

## **Evaluation Criteria**
Your submission will be scored based on the following:
1. **Object-Oriented Programming Principles** (Microsoft Standards)
2. **Coding Principles:** SOLID, DRY, KISS, Anti-Patterns, YAGNI
3. **Naming Conventions & Coding Standards** (Microsoft Standards)
4. Does your code compile out of the box?
5. Does your submission run out of the box?
6. Did we need to fix bugs before reviewing your code?
7. Were nuances around your submission documented?
8. Did you complete all tasks in the assessment?

## **Environment**
We will run your submission on a **vanilla install of Visual Studio 2022 Community Edition**.

### **Tech Stack Requirements**
- **Framework:** Microsoft .NET 8
- **Language:** C#
- **Project Type:** Blazor Server
- **Storage:** SQLite
- **Source Control:** Git

### **Submission Instructions**
1. Fork our repository to your Git account.
2. Complete the assessment on your forked repo.
3. Submit your work via a pull request or provide access to a public repository.

---

## **Task 1: Threading & Data Persistence**

### **Boilerplate Code**
- `TechAptV1.Client/Components/Pages/Threading.razor`
- `TechAptV1.Client/Services/ThreadingService.cs`
- `TechAptV1.Client/Services/DataService.cs`
- `TechAptV1.Client/Models/Number.cs`

### **Required Functionality**

#### **1. Threading.razor – Data Computation UI**
- **Start Button:**
  - Start the data computation process while adhering to UI best practices.
  - Disable the Start button during computation.
  - Enable the Save button only if computation is completed and data is available to save.
- **Save Button:**
  - Save the computed data to an SQLite database.

#### **2. ThreadingService.cs – Compute Data (Business Logic Requirements)**
- Create a thread to randomly pick **odd numbers** and add them to a shared global variable.
- Create a second thread to calculate **prime numbers**, negate them, and add them to the shared global variable.
- Once the shared global variable contains **2,500,000** entries:
  - Create a third thread to pick **even numbers** and add them to the shared global variable.
- Stop all threads when the shared global variable contains exactly **10,000,000** entries.
- Sort the global list in ascending order.
- Display the total count of numbers, odd numbers, and even numbers.
- Retain a handle on the list for user access and saving.
- The shared global variable must not exceed **10,000,000** entries.

#### **3. DataService.cs – Save Data (Persistence Requirements)**
- SQLite table structure:
  ```sql
  CREATE TABLE "Number" (
      "Value" INTEGER NOT NULL,
      "IsPrime" INTEGER NOT NULL DEFAULT 0
  );
  ```
- Efficiently insert the global list into the SQLite table.

---

## **Task 2: Results & Data Retrieval**

### **Boilerplate Code**
- `TechAptV1.Client/Components/Pages/Results.razor`
- `TechAptV1.Client/Services/DataService.cs`
- `TechAptV1.Client/Models/Number.cs`

### **Required Functionality**

#### **1. Results.razor – Data Computation Results UI**
- **Display Table:**
  - Show the top 20 results from the saved data generated in Task 1.
- **DownloadXml Button:**
  - Retrieve all records from the `Number` table.
  - Serialize the records to XML.
  - Provide the XML file for download.
- **DownloadBinary Button:**
  - Retrieve all records from the `Number` table.
  - Serialize the record columns to a binary format (`.bin`).
  - Provide the binary file for download.
