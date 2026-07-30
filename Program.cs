// Ahmed Ebrahim Abdelhamid  Lab ID = 5
// ================================================================================
//
//=================================== Part A ======================================
// A.1:
//    Instructors.AssignedCourseName          : PRESENT (expected — Block 3 removes it)
//
// A.3:
//    Table Students                          : FOUND (4 row(s))
//    Migration history                       : 1 migration(s) applied
//
// ================================================================================
// 
//=================================== Part C ======================================
// Derived GPA is : 3.0 + (( 5 mod 7) * 0.1) = 3.5
//
//
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace StudentPortalConsole
{
   
    public class Student
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        public int YearOfStudy { get; set; }
        public double Gpa { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string CourseName { get; set; } = "";

        public int Credits { get; set; }


        public int? InstructorId { get; set; }
        public Instructor? Instructor { get; set; } = null!;
    }

    public class Instructor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        public int YearsOfExperience { get; set; }


        //public string? AssignedCourseName { get; set; }


        public List<Course> Courses { get; set; } = new();
    }

    public class StudentPortalContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          
            optionsBuilder.UseSqlServer(
                "Server=.;Database=ITI_StudentPortal;Trusted_Connection=True;TrustServerCertificate=True;")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging(); 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100); // Fluent Api 

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);
        }


        internal class Program
        {

            static async Task Main(string[] args)
            {

                using (var context = new StudentPortalContext())
                {
                    // Day-ready proof: this runs as-is against Session 13's
                    // database, before any TODO below is completed.
                    Console.WriteLine("Students currently in the database:");
                    foreach (var student in await context.Students.ToListAsync())
                    {
                        Console.WriteLine($"  {student.FullName} — Year {student.YearOfStudy}, GPA {student.Gpa:F2}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("===========================================================");

                    Console.WriteLine("======================== Part B ===========================");
                    Console.WriteLine("===========================================================");
                    Console.WriteLine("*************************** B.1 ***************************");

                    var s1 = await context.Students.FirstAsync(x => x.Id == 1);
                    s1.Gpa = 3.99;
                    Console.WriteLine(s1.Gpa);
                    // the GPA of the student s1 is updated at C# not in database
                    //Console.WriteLine("===========================================================");
                    //Console.WriteLine("*************************** B.2 ***************************");

                    //var instructors = await context.Instructors.ToListAsync();
                    //foreach (var i in instructors)
                    //    Console.WriteLine($"{i.FullName}: {i.Courses.Count}");
                    // each instructer will print 0 courses and we didnt use Include(i => i.Courses)

                    Console.WriteLine("*************************** B.3 ***************************");

                    var s = await context.Students.AsNoTracking().FirstAsync();
                    s.Gpa = 2.0;
                    await context.SaveChangesAsync();
                    // nothing will change bec we used .AsNoTracking() that mean i wont change any data
                    // but he used here s.Gpa= 2.0 so it wont change any  thing and there is no need also for this
                    // await context.SaveChangesAsync();

                    Console.WriteLine("===========================================================");
                    Console.WriteLine("======================== Part C ===========================");
                    Console.WriteLine("===========================================================");
                    Console.WriteLine("*************************** C.1 ***************************");

                    var Nada = await context.Students.FirstOrDefaultAsync(s => s.FullName == "Nada Samir");
                    if (Nada != null)
                    {
                        Console.WriteLine($"Nada's Current GPA: {Nada.Gpa}");
                    }

                    // it printed: Nada's Current GPA: 3.9

                    Console.WriteLine("*************************** C.2 ***************************");

                    if (Nada!= null)
                    {

                        Nada.Gpa = 3.5;
                        Console.WriteLine($"Nada's GPA After update is: {Nada.Gpa}"); 
                    }

                    // its differ bec we didnt use SaveChangesAsync() to update the dataBase 
                    await context.SaveChangesAsync();

                    // EF core take a snapshot change tracker to feach when any thing change 
                    // when change the gpa it compare between the current values and the snapshot we can see the gpa only changed 

                    Console.WriteLine("*************************** C.4 ***************************");

                    //var newStudent = new Student
                    //{
                    //    FullName = "Ahmed Ebrahim",
                    //    YearOfStudy = 2,
                    //    Gpa = 3.5 // My Derived GPA

                    //};


                    //Console.WriteLine($"Student Id in C# before  Save: {newStudent.Id}");
                    //await context.Students.AddAsync(newStudent);
                    //await context.SaveChangesAsync();
                    //Console.WriteLine($"Student Id in DB after Save: {newStudent.Id}");
                    //Console.WriteLine("my Student Add");
                    Console.WriteLine("*************************** C.5 ***************************");

                    var myStudent = context.Students.FirstOrDefault(s => s.FullName == "Ahmed Ebrahim");
                    //if (myStudent != null)
                    //{
                    //    myStudent.YearOfStudy = 3;
                    //    Console.WriteLine($"Updated {myStudent.FullName}'s YearOfStudy is now: {myStudent.YearOfStudy}");

                    //}
                    //await context.SaveChangesAsync();

                    //  I verfied it
                    Console.WriteLine("*************************** C.6 ***************************");


                    if (myStudent != null)
                    {
                        context.Students.Remove(myStudent);
                        context.SaveChanges();
                        Console.WriteLine("removed me ");
                    }
                    Console.WriteLine("===========================================================");
                    Console.WriteLine("======================== Part D ===========================");
                    Console.WriteLine("===========================================================");
                    Console.WriteLine();
                    Console.WriteLine("*************************** D.4 ***************************");

                    // 1- It changes the column from nvarchar(max) to nvarchar(100)

                    // 2- nullable: false
                    //    oldNullable: is not present.
                    //    This means the old column was already not null,

                    // 3- A row with FullName longer than 100 characters.

                    // 4- 0 

                    Console.WriteLine("*************************** D.6 ***************************");
                    //try
                    //{
                    //    Student student = new Student
                    //    {
                    //        FullName = null!,
                    //        YearOfStudy = 2,
                    //        Gpa = 3.5
                    //    };

                    //    context.Students.Add(student);

                    //    await context.SaveChangesAsync();
                    //}
                    //catch (DbUpdateException ex)
                    //{
                    //    Console.WriteLine("Database rejected the NULL FullName.");
                    //    Console.WriteLine(ex.Message);
                    //    //An error occurred while saving the entity changes. See the inner exception for details.
                    //}

                    Console.WriteLine("===========================================================");
                    Console.WriteLine("======================== Part E ===========================");
                    Console.WriteLine("===========================================================");
                    Console.WriteLine("*************************** E.4 ***************************");
                    // 1)
                    //      1. Drop  AssignedCourseName
                    //      2. Add  InstructorId column to the Courses table
                    //      3. Create an index on Courses.InstructorId
                    //      4. Add foreign key from Courses.InstructorId
                    //         to Instructors.Id using  DeleteBehavior.SetNull

                    // 2) DropColumn("AssignedCourseName").

                    // 3) copy the old data to InstructorId then delete AssignedCourseName.

                    Console.WriteLine("*************************** E.5 ***************************");

                    var hamdy = await context.Instructors.FirstOrDefaultAsync(i => i.FullName == "Hamdy");
                    var webCourse = await context.Courses.FirstOrDefaultAsync(c => c.CourseName == "Web Development Using .NET");

                    if (hamdy != null && webCourse != null)
                    {
                        webCourse.InstructorId = hamdy.Id;
                        await context.SaveChangesAsync();
                        Console.WriteLine($"Course {webCourse.CourseName} has been assigned to {hamdy.FullName}");
                    }

                    Console.WriteLine("*************************** E.7 ***************************");
                    //try {
                    //    var exE7 = new Course {
                    //        CourseName = "Ai And Machine Learning",
                    //        Credits = 3,
                    //        InstructorId = 999 };

                    //    await context.Courses.AddAsync(exE7);
                    //    await context.SaveChangesAsync();
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //    //An error occurred while saving the entity changes. See the inner exception for details.

                    //}


                    Console.WriteLine("===========================================================");
                    Console.WriteLine("======================== Part F ===========================");
                    Console.WriteLine("===========================================================");
                    Console.WriteLine("*************************** F.1 ***************************");

                    //var extraCourses = new List<Course> { //  Adding 4 Courses
                    //    new Course {
                    //        CourseName = "Flutter",
                    //        Credits=3,
                    //        InstructorId=hamdy.Id
                    //    },
                    //    new Course
                    //    {
                    //        CourseName="Ai And Machine Learning",
                    //        Credits=3,
                    //        InstructorId=hamdy.Id
                    //    },
                    //    new Course
                    //    {
                    //        CourseName="Cyper",
                    //        Credits=2,
                    //        InstructorId=hamdy.Id

                    //    },
                    //    new Course
                    //    {
                    //        CourseName ="C# Advanced",
                    //        Credits=2,
                    //        InstructorId=hamdy.Id
                    //    }
                    //};

                    //await context.Courses.AddRangeAsync(extraCourses);
                    //await context.SaveChangesAsync();



                    Console.WriteLine("*************************** F.3 ***************************");

                    //using (var context1 = new StudentPortalContext())
                    //{
                    //    var instructors = await context1.Instructors.ToListAsync();

                    //    foreach (var instructor in instructors)
                    //    {
                    //        Console.WriteLine($"{instructor.FullName} : {instructor.Courses.Count}");
                    //    }
                    //}
                    // 1 Query

                    Console.WriteLine("*************************** F.4 ***************************");

                    //using (var context1 = new StudentPortalContext())
                    //{
                    //    var instructors = await context1.Instructors
                    //        .Include(i => i.Courses)
                    //        .ToListAsync();

                    //    foreach (var instructor in instructors)
                    //    {
                    //        Console.WriteLine(instructor.FullName);

                    //        foreach (var course in instructor.Courses)
                    //        {
                    //            Console.WriteLine($"   {course.CourseName}");
                    //        }
                    //    }
                    //}
                    // 1 Query


                    Console.WriteLine("*************************** F.5 ***************************");
                    // Include uses  left join so one instructor appears once for each course.
                    // EF Core removes the duplicate instructor  and fills the Courses list with the related courses.


                    Console.WriteLine("*************************** F.6 ***************************");

                    //using (var context1 = new StudentPortalContext())
                    //{
                    //    var instructor = await context1.Instructors
                    //        .FirstAsync(i => i.FullName == "Hamdy");

                    //    Console.WriteLine($"Before explicit loading: {instructor.Courses.Count}");

                    //    await context.Entry(instructor)
                    //        .Collection(i => i.Courses)
                    //        .LoadAsync();

                    //    Console.WriteLine($"After explicit loading: {instructor.Courses.Count}");
                    //}
                    // Before loading = 0
                    // After loading = 5
                    // 1 query for instructor.
                    // 1 query for Courses collection explicitly.

                    Console.WriteLine("*************************** F.7 ***************************");

                    var exF7 = await context.Students
                                    .AsNoTracking()
                                    .ToListAsync();

                    exF7[0].Gpa = 1.0;

                    await context.SaveChangesAsync();

                    // Nothing changed in SSMS.
                    // AsNoTracking() make change tracker dosent make any snapshot and dont track any change bec it will read only


                    /*
                    //===========================================================
                    //                           Part G
                    //===========================================================
                    1) Derived Values:
                            1. GPA = 3.5
                                     
                            2. Extra Courses Count = 4 Courses
                                              
                            3. OnDelete Behavior: DeleteBehavior.SetNull
                     2) on deleting an instructor the instructorId will be null at course table bec we used setnull that make any related data with this instractor make it null          

                     3) i used Remove-Migration & Get-Migration

                     4) Multiple Enumeration and N+1 are similar because both send extra SQL queries(مش عارف المفروض اي تاني اقوله بمعني اخر مش فاهم السؤال وفصلت 0_0)


                    */

                }



                Console.WriteLine();
                    Console.WriteLine("Done.");
                
            }
        }


    }
}
