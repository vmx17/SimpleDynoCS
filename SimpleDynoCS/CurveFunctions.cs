using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq; // DC 21JAN16 - this needed to be added to support new curve fitting, specifically the ToList part of line "Return answer.ToList()"
using Microsoft.VisualBasic;

namespace SimpleDyno
{
    static class CurveFunctions
    {
        // The function.
        public static double F(List<double> coeffs, double x)
        {
            double total = 0d;
            double x_factor = 1d;
            for (int i = 0, loopTo = coeffs.Count - 1; i <= loopTo; i++)
            {
                total += x_factor * coeffs[i];
                x_factor *= x;
            }
            return total;
        }

        // Return the error squared.
        public static double ErrorSquared(List<PointF> points, List<double> coeffs)
        {
            double total = 0d;
            foreach (PointF pt in points)
            {
                double dy = (double)pt.Y - F(coeffs, (double)pt.X);
                total += dy * dy;
            }
            return total;
        }

        // Find the least squares linear fit.
        // DC 21JAN16 - FindPolynomialLeastSquaresFit needs to be modified to handle the original SimpleDyno arrays of raw data
        // This is the original function
        public static List<double> FindPolynomialLeastSquaresFit(List<PointF> points, int degree)
        {
            // Allocate space for (degree + 1) equations with 
            // (degree + 2) terms each (including the constant term).
            var coeffs = new double[degree + 1, degree + 1 + 1];

            // Calculate the coefficients for the equations.
            for (int j = 0, loopTo = degree; j <= loopTo; j++)
            {
                // Calculate the coefficients for the jth equation.

                // Calculate the constant term for this equation.
                coeffs[j, degree + 1] = 0d;
                foreach (PointF pt in points)
                    coeffs[j, degree + 1] -= Math.Pow((double)pt.X, j) * (double)pt.Y;

                // Calculate the other coefficients.
                for (int a_sub = 0, loopTo1 = degree; a_sub <= loopTo1; a_sub++)
                {
                    // Calculate the dth coefficient.
                    coeffs[j, a_sub] = 0d;
                    foreach (PointF pt in points)
                        coeffs[j, a_sub] -= Math.Pow((double)pt.X, a_sub + j);
                }
            }

            // Solve the equations.
            double[] answer = GaussianElimination(coeffs);

            // Return the result converted into a List(Of Double).
            return answer.ToList();
        }
        // This is the revised function for use with SimpleDyno
        public static bool FindPolynomialLeastSquaresFit_NEW(ref double[] SentX, ref double[] SentY, ref double[] SentFY, int degree)
        {

            int TempCount; // added for SD

            // Allocate space for (degree + 1) equations with 
            // (degree + 2) terms each (including the constant term).
            var coeffs = new double[degree + 1, degree + 1 + 1];

            // Calculate the coefficients for the equations.
            for (int j = 0, loopTo = degree; j <= loopTo; j++)
            {
                // Calculate the coefficients for the jth equation.

                // Calculate the constant term for this equation.
                coeffs[j, degree + 1] = 0d;
                // DC 21JAN16 - the following loop replaced
                // For Each pt As PointF In points
                // coeffs(j, degree + 1) -= Math.Pow(pt.X, j) * pt.Y
                // Next pt

                var loopTo1 = Information.UBound(SentX);
                for (TempCount = 0; TempCount <= loopTo1; TempCount++)
                    coeffs[j, degree + 1] -= Math.Pow(SentX[TempCount], j) * SentY[TempCount];


                // Calculate the other coefficients.
                for (int a_sub = 0, loopTo2 = degree; a_sub <= loopTo2; a_sub++)
                {
                    // Calculate the dth coefficient.
                    coeffs[j, a_sub] = 0d;
                    // DC 21JAN16 The following Loop replaced
                    // For Each pt As PointF In points
                    // coeffs(j, a_sub) -= Math.Pow(pt.X, a_sub + j)
                    // Next pt
                    var loopTo3 = Information.UBound(SentX);
                    for (TempCount = 0; TempCount <= loopTo3; TempCount++)
                        coeffs[j, a_sub] -= Math.Pow(SentX[TempCount], a_sub + j);
                }
            }

            // Solve the equations.
            double[] answer = GaussianElimination(coeffs);
            // Original
            // Return the result converted into a List(Of Double).
            // Return answer.ToList()
            // New - Use F function to calculate the fy values

            var loopTo4 = Information.UBound(SentX);
            for (TempCount = 0; TempCount <= loopTo4; TempCount++)
                SentFY[TempCount] = F(answer.ToList(), SentX[TempCount]);
            // FOR NOW JUST RETURN TRUE

            return true;
        }

        // Perform Gaussian elimination on these coefficients.
        // Return the array of values that gives the solution.
        private static double[] GaussianElimination(double[,] coeffs)
        {
            int max_equation = coeffs.GetUpperBound(0);
            int max_coeff = coeffs.GetUpperBound(1);
            for (int i = 0, loopTo = max_equation; i <= loopTo; i++)
            {
                // Use equation_coeffs(i, i) to eliminate the ith
                // coefficient in all of the other equations.

                // Find a row with non-zero ith coefficient.
                if (coeffs[i, i] == 0d)
                {
                    for (int j = i + 1, loopTo1 = max_equation; j <= loopTo1; j++)
                    {
                        // See if this one works.
                        if (coeffs[j, i] != 0d)
                        {
                            // This one works. Swap equations i and j.
                            // This starts at k = i because all
                            // coefficients to the left are 0.
                            for (int k = i, loopTo2 = max_coeff; k <= loopTo2; k++)
                            {
                                double temp = coeffs[i, k];
                                coeffs[i, k] = coeffs[j, k];
                                coeffs[j, k] = temp;
                            }
                            break;
                        }
                    }
                }

                // Make sure we found an equation with
                // a non-zero ith coefficient.
                double coeff_i_i = coeffs[i, i];
                if (coeff_i_i == 0d)
                {
                    throw new ArithmeticException(string.Format("There is no unique solution for these points.", coeffs.GetUpperBound(0) - 1));

                }

                // Normalize the ith equation.
                for (int j = i, loopTo3 = max_coeff; j <= loopTo3; j++)
                    coeffs[i, j] /= coeff_i_i;

                // Use this equation value to zero out
                // the other equations' ith coefficients.
                for (int j = 0, loopTo4 = max_equation; j <= loopTo4; j++)
                {
                    // Skip the ith equation.
                    if (j != i)
                    {
                        // Zero the jth equation's ith coefficient.
                        double coef_j_i = coeffs[j, i];
                        for (int d = 0, loopTo5 = max_coeff; d <= loopTo5; d++)
                            coeffs[j, d] -= coeffs[i, d] * coef_j_i;
                    }
                }
            }

            // At this point, the ith equation contains
            // 2 non-zero entries:
            // The ith entry which is 1
            // The last entry coeffs(max_coeff)
            // This means Ai = equation_coef(max_coeff).
            var solution = new double[max_equation + 1];
            for (int i = 0, loopTo6 = max_equation; i <= loopTo6; i++)
                solution[i] = coeffs[i, max_coeff];

            // Return the solution values.
            return solution;
        }
    }
}