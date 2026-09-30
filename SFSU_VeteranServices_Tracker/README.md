```markdown
## Veteran-Services-Tracker

## SETUP REQUIREMENTS:

This project is being developed using **C#, .NET MAUI, and XAML**.

Before running the project, make sure the following are installed:

* Visual Studio 2022 or newer
* .NET MAUI workload
* .NET SDK
* Git

When installing Visual Studio, make sure the **.NET Multi-platform App UI development** workload is selected.

The project should be opened and run using Visual Studio.


**Background**:

The Veteran Services Tracker is an application designed to help a university Veteran Services office keep track of students who enter and use the Veterans Corner.

The purpose of this project is to create a simple check-in application that can record student information and keep track of visits to the Veterans Corner.

Because this project does not have access to the university's actual student database, the first version of the application will use local or mock data.


**Task**:

Develop a .NET MAUI application that allows students to check into the Veterans Corner.

The student will enter their Student ID, full name, and select their current status.

The available status options include:

* Veteran
* Active Duty
* Reserve
* Dependent
* Civilian

Before completing the check-in process, the student must read and confirm that they have read the Veterans Corner **Code of Conduct**.

After the information has been entered, the application will validate the information and create a check-in record.

The application will also allow previous check-in records to be displayed.


**Requirements**:

The project must be written using **C# and .NET MAUI**.

The application should demonstrate programming concepts learned during MSSA including:

* Variables
* Conditional statements
* Loops
* Methods
* Classes
* Objects
* Object-Oriented Programming
* Data structures
* Input validation
* XAML
* .NET MAUI controls

The application must allow the user to:

* Enter a Student ID
* Enter a full name
* Select a student status
* Read the Code of Conduct
* Confirm that the Code of Conduct has been read
* Complete a student check-in
* Store a check-in record
* View previous check-in records

The application should not allow a student to check in if required information is missing.

The student must also confirm that they have read the Code of Conduct before the check-in can be completed.


**Application Flow**:

The normal student check-in process will follow:

```

Open Application
|
v
Enter Student ID
|
v
Enter Full Name
|
v
Select Student Status
|
v
Read Code of Conduct
|
v
Accept Code of Conduct
|
v
Press Check In
|
v
Validate Information
|
v
Create Check-In Record
|
v
Display Confirmation

````


**Data Structures**:

The application will use data structures to store student and check-in information.

A `List` can be used to store check-in records.

```csharp
List<CheckInRecord> checkInRecords;
````

An `ObservableCollection` may be used when displaying the check-in records inside a .NET MAUI `CollectionView`.

```csharp
ObservableCollection<CheckInRecord> checkInRecords;
```

A `Dictionary` may be added later if the application needs to search for students using their Student ID.

```csharp
Dictionary<string, Student> students;
```

**Project Structure**:

The planned project structure is:

```
VeteranServicesTracker
|
|-- Models
|   |-- Student.cs
|   |-- CheckInRecord.cs
|
|-- ViewModels
|   |-- CheckInViewModel.cs
|   |-- HistoryViewModel.cs
|
|-- Views
|   |-- CheckInPage.xaml
|   |-- CodeOfConductPage.xaml
|   |-- HistoryPage.xaml
|
|-- App.xaml
|-- App.xaml.cs
|-- AppShell.xaml
|-- AppShell.xaml.cs
|-- MainPage.xaml
|-- MainPage.xaml.cs
```

The first version may use a simpler structure while the basic functionality of the application is being developed.

**Student Model**:

The `Student` class will store information about the student checking in.

```csharp
public class Student
{
    public string StudentId { get; set; }
    public string FullName { get; set; }
    public string Status { get; set; }
}
```

**Check-In Record**:

The `CheckInRecord` class will store the student and the date and time they checked in.

```csharp
public class CheckInRecord
{
    public Student Student { get; set; }
    public DateTime CheckInTime { get; set; }
}
```

**Pages**:

The application will contain multiple pages.

* **Check-In Page**

  * Enter Student ID
  * Enter full name
  * Select student status
  * Confirm Code of Conduct
  * Check into the Veterans Corner

* **Code of Conduct Page**

  * Displays the Veterans Corner Code of Conduct
  * Allows the student to read the rules before checking in

* **Check-In History Page**

  * Displays previous student check-ins
  * Displays student name
  * Displays student status
  * Displays check-in time

**Example Check-In History**:

```
Clark Batungbakal
Veteran
10:30 AM

John Smith
Active Duty
11:15 AM
```

**How to Run**:

1. Open the project in Visual Studio.
2. Make sure the .NET MAUI workload is installed.
3. Select the platform that the application will run on.
4. Build the solution.
5. Run the application.
6. Enter the student's information.
7. Select the student's status.
8. Read and accept the Code of Conduct.
9. Press the **Check In** button.
10. Verify that the student's check-in was completed.

**Testing**:

The application should be tested for the following:

* Student can enter a Student ID
* Student can enter their full name
* Student can select a status
* Student can read the Code of Conduct
* Student cannot check in without accepting the Code of Conduct
* Student cannot check in if required information is missing
* Valid check-in creates a check-in record
* Check-in time is recorded
* Multiple check-in records can be stored
* Check-in history displays correctly

**Future Development**:

The first version of the Veteran Services Tracker will be a local application.

After the basic version is completed, the application may be expanded to include:

* Microsoft Azure
* Cloud database
* Persistent student records
* Staff login
* Admin accounts
* Student search
* Check-in and check-out tracking
* Daily visitor totals
* Monthly visitor statistics
* Reports
* Authentication
* Improved user interface

The long-term goal is to expand the application into a cloud-based project that demonstrates concepts learned through college and MSSA.

**NOTE:**

The first version of the project will focus on completing the basic student check-in functionality before adding cloud services or more advanced features.

Mock or local data will be used because the application does not have access to the actual university student database.

```
```
