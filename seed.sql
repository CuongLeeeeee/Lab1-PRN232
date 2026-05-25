-- ============================================================
-- StudentPortal – Full Seed Script
-- 5 Semesters | 10 Subjects | 20 Courses | 50 Students
-- 20 CourseSubject links | 500 Enrollments
-- Run against: StudentPortalDb (or StudentPortalDb_Dev)
-- Assumes EF migrations have already been applied.
-- ============================================================

SET NOCOUNT ON;
GO

-- ─────────────────────────────────────────────────────────────
-- 0. CLEAN existing data (order respects FK constraints)
-- ─────────────────────────────────────────────────────────────
DELETE FROM Enrollments;
DELETE FROM CourseSubjects;
DELETE FROM Courses;
DELETE FROM Students;
DELETE FROM Subjects;
DELETE FROM Semesters;

-- Reset identity columns
DBCC CHECKIDENT ('Enrollments', RESEED, 0);
DBCC CHECKIDENT ('Courses',     RESEED, 0);
DBCC CHECKIDENT ('Students',    RESEED, 0);
DBCC CHECKIDENT ('Subjects',    RESEED, 0);
DBCC CHECKIDENT ('Semesters',   RESEED, 0);
GO

-- ─────────────────────────────────────────────────────────────
-- 1. SEMESTERS (5 rows)
-- ─────────────────────────────────────────────────────────────
INSERT INTO Semesters (SemesterName, StartDate, EndDate) VALUES
('Spring 2023', '2023-01-09', '2023-05-26'),
('Fall 2023',   '2023-08-21', '2023-12-22'),
('Spring 2024', '2024-01-08', '2024-05-24'),
('Fall 2024',   '2024-08-19', '2024-12-20'),
('Spring 2025', '2025-01-06', '2025-05-23');
GO

-- ─────────────────────────────────────────────────────────────
-- 2. SUBJECTS (10 rows)
-- ─────────────────────────────────────────────────────────────
INSERT INTO Subjects (SubjectCode, SubjectName, Credit) VALUES
('MAD101',  'Mobile Application Development',          3),
('PRN212',  'Basic Cross-Platform Application',        3),
('PRN231',  'Advanced Cross-Platform Application',     3),
('PRN232',  'ASP.NET Web API Development',             3),
('PRJ301',  'Java Web Application Development',        3),
('SWD392',  'Software Architecture & Design',          3),
('SWR302',  'Software Requirement Engineering',        2),
('SWP391',  'Software Development Project',            3),
('DBD311',  'Database Design',                         3),
('IOT102',  'Internet of Things',                      3);
GO

-- ─────────────────────────────────────────────────────────────
-- 3. COURSES (20 rows – 4 per semester)
-- ─────────────────────────────────────────────────────────────
INSERT INTO Courses (CourseName, SemesterId) VALUES
-- Semester 1 – Spring 2023
('MAD101 – Mobile App Development (SP23)',        1),
('PRN212 – Cross-Platform Basics (SP23)',          1),
('DBD311 – Database Design (SP23)',               1),
('SWR302 – Requirements Engineering (SP23)',      1),
-- Semester 2 – Fall 2023
('PRN231 – Cross-Platform Advanced (FA23)',        2),
('PRN232 – ASP.NET Web API (FA23)',               2),
('SWD392 – Software Architecture (FA23)',          2),
('IOT102 – Internet of Things (FA23)',            2),
-- Semester 3 – Spring 2024
('PRJ301 – Java Web Application (SP24)',           3),
('SWP391 – Software Development Project (SP24)',  3),
('MAD101 – Mobile App Development (SP24)',        3),
('DBD311 – Database Design (SP24)',               3),
-- Semester 4 – Fall 2024
('PRN232 – ASP.NET Web API (FA24)',               4),
('PRN231 – Cross-Platform Advanced (FA24)',        4),
('SWD392 – Software Architecture (FA24)',          4),
('SWR302 – Requirements Engineering (FA24)',      4),
-- Semester 5 – Spring 2025
('PRJ301 – Java Web Application (SP25)',           5),
('SWP391 – Software Development Project (SP25)',  5),
('IOT102 – Internet of Things (SP25)',            5),
('PRN212 – Cross-Platform Basics (SP25)',          5);
GO

-- ─────────────────────────────────────────────────────────────
-- 4. COURSE-SUBJECT links (1 subject per course, matching name)
-- ─────────────────────────────────────────────────────────────
INSERT INTO CourseSubjects (CourseId, SubjectId) VALUES
( 1,  1),  -- MAD101 (SP23)  → MAD101
( 2,  2),  -- PRN212 (SP23)  → PRN212
( 3,  9),  -- DBD311 (SP23)  → DBD311
( 4,  7),  -- SWR302 (SP23)  → SWR302
( 5,  3),  -- PRN231 (FA23)  → PRN231
( 6,  4),  -- PRN232 (FA23)  → PRN232
( 7,  6),  -- SWD392 (FA23)  → SWD392
( 8, 10),  -- IOT102 (FA23)  → IOT102
( 9,  5),  -- PRJ301 (SP24)  → PRJ301
(10,  8),  -- SWP391 (SP24)  → SWP391
(11,  1),  -- MAD101 (SP24)  → MAD101
(12,  9),  -- DBD311 (SP24)  → DBD311
(13,  4),  -- PRN232 (FA24)  → PRN232
(14,  3),  -- PRN231 (FA24)  → PRN231
(15,  6),  -- SWD392 (FA24)  → SWD392
(16,  7),  -- SWR302 (FA24)  → SWR302
(17,  5),  -- PRJ301 (SP25)  → PRJ301
(18,  8),  -- SWP391 (SP25)  → SWP391
(19, 10),  -- IOT102 (SP25)  → IOT102
(20,  2);  -- PRN212 (SP25)  → PRN212
GO

-- ─────────────────────────────────────────────────────────────
-- 5. STUDENTS (50 rows – realistic Vietnamese names)
-- ─────────────────────────────────────────────────────────────
INSERT INTO Students (FullName, Email) VALUES
('Nguyen Van An',        'an.nguyenvan@fpt.edu.vn'),
('Tran Thi Bich',        'bich.tranthI@fpt.edu.vn'),
('Le Van Cuong',         'cuong.levan@fpt.edu.vn'),
('Pham Thi Dung',        'dung.phamthi@fpt.edu.vn'),
('Hoang Van Em',         'em.hoangvan@fpt.edu.vn'),
('Bui Thi Phuong',       'phuong.buithi@fpt.edu.vn'),
('Do Van Giang',         'giang.dovan@fpt.edu.vn'),
('Vo Thi Hoa',           'hoa.vothi@fpt.edu.vn'),
('Dang Van Hung',        'hung.dangvan@fpt.edu.vn'),
('Nguyen Thi Lan',       'lan.nguyenthi@fpt.edu.vn'),
('Tran Van Long',        'long.tranvan@fpt.edu.vn'),
('Le Thi Mai',           'mai.lethi@fpt.edu.vn'),
('Pham Van Nam',         'nam.phamvan@fpt.edu.vn'),
('Hoang Thi Oanh',       'oanh.hoangthi@fpt.edu.vn'),
('Bui Van Phuc',         'phuc.buivan@fpt.edu.vn'),
('Do Thi Quynh',         'quynh.dothi@fpt.edu.vn'),
('Vo Van Sang',          'sang.vovan@fpt.edu.vn'),
('Dang Thi Thu',         'thu.dangthi@fpt.edu.vn'),
('Nguyen Van Toan',      'toan.nguyenvan2@fpt.edu.vn'),
('Tran Thi Uyen',        'uyen.tranthI@fpt.edu.vn'),
('Le Van Vinh',          'vinh.levan@fpt.edu.vn'),
('Pham Thi Xuan',        'xuan.phamthi@fpt.edu.vn'),
('Hoang Van Yen',        'yen.hoangvan@fpt.edu.vn'),
('Bui Thi Zung',         'zung.buithi@fpt.edu.vn'),
('Do Van Anh',           'anh.dovan@fpt.edu.vn'),
('Vo Thi Bao',           'bao.vothi@fpt.edu.vn'),
('Dang Van Chi',         'chi.dangvan@fpt.edu.vn'),
('Nguyen Thi Dao',       'dao.nguyenthi@fpt.edu.vn'),
('Tran Van Duc',         'duc.tranvan@fpt.edu.vn'),
('Le Thi Gia',           'gia.lethi@fpt.edu.vn'),
('Pham Van Hai',         'hai.phamvan@fpt.edu.vn'),
('Hoang Thi Ivy',        'ivy.hoangthi@fpt.edu.vn'),
('Bui Van Khoa',         'khoa.buivan@fpt.edu.vn'),
('Do Thi Linh',          'linh.dothi@fpt.edu.vn'),
('Vo Van Minh',          'minh.vovan@fpt.edu.vn'),
('Dang Thi Ngoc',        'ngoc.dangthi@fpt.edu.vn'),
('Nguyen Van Oanh',      'oanh.nguyenvan@fpt.edu.vn'),
('Tran Thi Phuong',      'phuong.tranthI@fpt.edu.vn'),
('Le Van Quan',          'quan.levan@fpt.edu.vn'),
('Pham Thi Rong',        'rong.phamthi@fpt.edu.vn'),
('Hoang Van Son',        'son.hoangvan@fpt.edu.vn'),
('Bui Thi Tam',          'tam.buithi@fpt.edu.vn'),
('Do Van Uy',            'uy.dovan@fpt.edu.vn'),
('Vo Thi Van',           'van.vothi@fpt.edu.vn'),
('Dang Van Xuan',        'xuan.dangvan@fpt.edu.vn'),
('Nguyen Thi Yen',       'yen.nguyenthi@fpt.edu.vn'),
('Tran Van Zach',        'zach.tranvan@fpt.edu.vn'),
('Le Thi Anh Tuyet',     'tuyet.lethi@fpt.edu.vn'),
('Pham Van Bao Long',    'baolong.phamvan@fpt.edu.vn'),
('Hoang Thi Cam Nhung',  'nhung.hoangthi@fpt.edu.vn');
GO

-- ─────────────────────────────────────────────────────────────
-- 6. ENROLLMENTS (500 rows)
--    Strategy: each student gets 10 distinct courses
--    Students 1–50, Courses 1–20 cycling with offsets
--    to ensure variety and no duplicate (StudentId, CourseId).
-- ─────────────────────────────────────────────────────────────

-- We use a CTE-based approach so the INSERT is clean SQL
-- without requiring a loop or procedural code.
-- Each student is assigned courses via deterministic offset:
--   course = ((student_index + course_slot * 3) % 20) + 1

WITH StudentSlots AS (
    SELECT
        s.StudentId,
        ((s.StudentId - 1 + slot.n * 3) % 20) + 1 AS CourseId,
        DATEADD(DAY,
            ABS(CHECKSUM(s.StudentId, slot.n)) % 30,
            '2023-01-09') AS EnrolledAt
    FROM Students s
    CROSS JOIN (
        SELECT 0 AS n UNION SELECT 1 UNION SELECT 2 UNION SELECT 3
        UNION SELECT 4 UNION SELECT 5 UNION SELECT 6 UNION SELECT 7
        UNION SELECT 8 UNION SELECT 9
    ) slot
)
INSERT INTO Enrollments (StudentId, CourseId, EnrolledAt)
SELECT StudentId, CourseId, EnrolledAt
FROM StudentSlots;
GO

-- ─────────────────────────────────────────────────────────────
-- 7. VERIFY counts
-- ─────────────────────────────────────────────────────────────
SELECT 'Semesters'     AS [Table], COUNT(*) AS [Rows] FROM Semesters
UNION ALL
SELECT 'Subjects',       COUNT(*) FROM Subjects
UNION ALL
SELECT 'Courses',        COUNT(*) FROM Courses
UNION ALL
SELECT 'CourseSubjects', COUNT(*) FROM CourseSubjects
UNION ALL
SELECT 'Students',       COUNT(*) FROM Students
UNION ALL
SELECT 'Enrollments',    COUNT(*) FROM Enrollments;
GO

-- ─────────────────────────────────────────────────────────────
-- 8. SAMPLE QUERIES to validate data quality
-- ─────────────────────────────────────────────────────────────

-- Enrollments per student (should all be 10)
SELECT s.StudentId, s.FullName, COUNT(e.EnrollmentId) AS EnrolledCourses
FROM Students s
JOIN Enrollments e ON e.StudentId = s.StudentId
GROUP BY s.StudentId, s.FullName
ORDER BY s.StudentId;

-- Enrollments per course
SELECT c.CourseId, c.CourseName, sem.SemesterName,
       COUNT(e.EnrollmentId) AS EnrolledStudents
FROM Courses c
JOIN Semesters sem ON sem.SemesterId = c.SemesterId
LEFT JOIN Enrollments e ON e.CourseId = c.CourseId
GROUP BY c.CourseId, c.CourseName, sem.SemesterName
ORDER BY c.CourseId;

-- Courses per semester
SELECT sem.SemesterName, COUNT(c.CourseId) AS TotalCourses
FROM Semesters sem
LEFT JOIN Courses c ON c.SemesterId = sem.SemesterId
GROUP BY sem.SemesterId, sem.SemesterName
ORDER BY sem.SemesterId;
GO
