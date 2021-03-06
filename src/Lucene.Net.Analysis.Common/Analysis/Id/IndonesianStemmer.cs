﻿namespace org.apache.lucene.analysis.id
{

	/*
	 * Licensed to the Apache Software Foundation (ASF) under one or more
	 * contributor license agreements.  See the NOTICE file distributed with
	 * this work for additional information regarding copyright ownership.
	 * The ASF licenses this file to You under the Apache License, Version 2.0
	 * (the "License"); you may not use this file except in compliance with
	 * the License.  You may obtain a copy of the License at
	 *
	 *     http://www.apache.org/licenses/LICENSE-2.0
	 *
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS,
	 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	 * See the License for the specific language governing permissions and
	 * limitations under the License.
	 */

	using org.apache.lucene.analysis.util;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.apache.lucene.analysis.util.StemmerUtil.*;

	/// <summary>
	/// Stemmer for Indonesian.
	/// <para>
	/// Stems Indonesian words with the algorithm presented in:
	/// <i>A Study of Stemming Effects on Information Retrieval in 
	/// Bahasa Indonesia</i>, Fadillah Z Tala.
	/// http://www.illc.uva.nl/Publications/ResearchReports/MoL-2003-02.text.pdf
	/// </para>
	/// </summary>
	public class IndonesianStemmer
	{
	  private int numSyllables;
	  private int flags;
	  private const int REMOVED_KE = 1;
	  private const int REMOVED_PENG = 2;
	  private const int REMOVED_DI = 4;
	  private const int REMOVED_MENG = 8;
	  private const int REMOVED_TER = 16;
	  private const int REMOVED_BER = 32;
	  private const int REMOVED_PE = 64;

	  /// <summary>
	  /// Stem a term (returning its new length).
	  /// <para>
	  /// Use <code>stemDerivational</code> to control whether full stemming
	  /// or only light inflectional stemming is done.
	  /// </para>
	  /// </summary>
	  public virtual int stem(char[] text, int length, bool stemDerivational)
	  {
		flags = 0;
		numSyllables = 0;
		for (int i = 0; i < length; i++)
		{
		  if (isVowel(text[i]))
		  {
			  numSyllables++;
		  }
		}

		if (numSyllables > 2)
		{
			length = removeParticle(text, length);
		}
		if (numSyllables > 2)
		{
			length = removePossessivePronoun(text, length);
		}

		if (stemDerivational_Renamed)
		{
		  length = stemDerivational(text, length);
		}
		return length;
	  }

	  private int stemDerivational(char[] text, int length)
	  {
		int oldLength = length;
		if (numSyllables > 2)
		{
			length = removeFirstOrderPrefix(text, length);
		}
		if (oldLength != length) // a rule is fired
		{
		  oldLength = length;
		  if (numSyllables > 2)
		  {
			  length = removeSuffix(text, length);
		  }
		  if (oldLength != length) // a rule is fired
		  {
			if (numSyllables > 2)
			{
				length = removeSecondOrderPrefix(text, length);
			}
		  }
		} // fail
		else
		{
		  if (numSyllables > 2)
		  {
			  length = removeSecondOrderPrefix(text, length);
		  }
		  if (numSyllables > 2)
		  {
			  length = removeSuffix(text, length);
		  }
		}
		return length;
	  }

	  private bool isVowel(char ch)
	  {
		switch (ch)
		{
		  case 'a':
		  case 'e':
		  case 'i':
		  case 'o':
		  case 'u':
			return true;
		  default:
			return false;
		}
	  }

	  private int removeParticle(char[] text, int length)
	  {
		if (StemmerUtil.EndsWith(text, length, "kah") || StemmerUtil.EndsWith(text, length, "lah") || StemmerUtil.EndsWith(text, length, "pun"))
		{
			numSyllables--;
			return length - 3;
		}

		return length;
	  }

	  private int removePossessivePronoun(char[] text, int length)
	  {
		if (StemmerUtil.EndsWith(text, length, "ku") || StemmerUtil.EndsWith(text, length, "mu"))
		{
		  numSyllables--;
		  return length - 2;
		}

		if (StemmerUtil.EndsWith(text, length, "nya"))
		{
		  numSyllables--;
		  return length - 3;
		}

		return length;
	  }

	  private int removeFirstOrderPrefix(char[] text, int length)
	  {
		if (StemmerUtil.StartsWith(text, length, "meng"))
		{
		  flags |= REMOVED_MENG;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 4);
		}

		if (StemmerUtil.StartsWith(text, length, "meny") && length > 4 && isVowel(text[4]))
		{
		  flags |= REMOVED_MENG;
		  text[3] = 's';
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (StemmerUtil.StartsWith(text, length, "men"))
		{
		  flags |= REMOVED_MENG;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (StemmerUtil.StartsWith(text, length, "mem"))
		{
		  flags |= REMOVED_MENG;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (StemmerUtil.StartsWith(text, length, "me"))
		{
		  flags |= REMOVED_MENG;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 2);
		}

		if (StemmerUtil.StartsWith(text, length, "peng"))
		{
		  flags |= REMOVED_PENG;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 4);
		}

		if (StemmerUtil.StartsWith(text, length, "peny") && length > 4 && isVowel(text[4]))
		{
		  flags |= REMOVED_PENG;
		  text[3] = 's';
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (StemmerUtil.StartsWith(text, length, "peny"))
		{
		  flags |= REMOVED_PENG;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 4);
		}

		if (StemmerUtil.StartsWith(text, length, "pen") && length > 3 && isVowel(text[3]))
		{
		  flags |= REMOVED_PENG;
		  text[2] = 't';
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 2);
		}

		if (StemmerUtil.StartsWith(text, length, "pen"))
		{
		  flags |= REMOVED_PENG;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (StemmerUtil.StartsWith(text, length, "pem"))
		{
		  flags |= REMOVED_PENG;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (StemmerUtil.StartsWith(text, length, "di"))
		{
		  flags |= REMOVED_DI;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 2);
		}

		if (StemmerUtil.StartsWith(text, length, "ter"))
		{
		  flags |= REMOVED_TER;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (StemmerUtil.StartsWith(text, length, "ke"))
		{
		  flags |= REMOVED_KE;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 2);
		}

		return length;
	  }

	  private int removeSecondOrderPrefix(char[] text, int length)
	  {
		if (StemmerUtil.StartsWith(text, length, "ber"))
		{
		  flags |= REMOVED_BER;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (length == 7 && StemmerUtil.StartsWith(text, length, "belajar"))
		{
		  flags |= REMOVED_BER;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (StemmerUtil.StartsWith(text, length, "be") && length > 4 && !isVowel(text[2]) && text[3] == 'e' && text[4] == 'r')
		{
		  flags |= REMOVED_BER;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 2);
		}

		if (StemmerUtil.StartsWith(text, length, "per"))
		{
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (length == 7 && StemmerUtil.StartsWith(text, length, "pelajar"))
		{
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 3);
		}

		if (StemmerUtil.StartsWith(text, length, "pe"))
		{
		  flags |= REMOVED_PE;
		  numSyllables--;
		  return StemmerUtil.deleteN(text, 0, length, 2);
		}

		return length;
	  }

	  private int removeSuffix(char[] text, int length)
	  {
		if (StemmerUtil.EndsWith(text, length, "kan") && (flags & REMOVED_KE) == 0 && (flags & REMOVED_PENG) == 0 && (flags & REMOVED_PE) == 0)
		{
		  numSyllables--;
		  return length - 3;
		}

		if (StemmerUtil.EndsWith(text, length, "an") && (flags & REMOVED_DI) == 0 && (flags & REMOVED_MENG) == 0 && (flags & REMOVED_TER) == 0)
		{
		  numSyllables--;
		  return length - 2;
		}

		if (StemmerUtil.EndsWith(text, length, "i") && !StemmerUtil.EndsWith(text, length, "si") && (flags & REMOVED_BER) == 0 && (flags & REMOVED_KE) == 0 && (flags & REMOVED_PENG) == 0)
		{
		  numSyllables--;
		  return length - 1;
		}
		return length;
	  }
	}

}