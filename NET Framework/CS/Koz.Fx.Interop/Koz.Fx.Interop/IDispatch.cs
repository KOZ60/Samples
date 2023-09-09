using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Koz.Fx.Interop
{
    /// <summary>
    /// �I�u�W�F�N�g�A���\�b�h�A����уv���p�e�B���A���������T�|�[�g����v���O���~���O�c�[���₻�̑��̃A�v���P�[�V�����Ɍ��J���܂��B
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00020400-0000-0000-C000-000000000046")]
    public interface IDispatch
    {
        /// <summary>
        /// �I�u�W�F�N�g���񋟂���^���C���^�[�t�F�C�X�̐� (0 �܂��� 1) ���擾���܂��B
        /// </summary>
        /// <param name="pctinfo">�I�u�W�F�N�g�ɂ��񋟂����^�^�C�v���C���^�[�t�F�C�X�̐����󂯎��ꏊ���w�肵�܂��B</param>
        [PreserveSig]
        void GetTypeInfoCount(out uint pctinfo);

        /// <summary>
        /// �I�u�W�F�N�g�̌^�����擾���܂��B���̌^�����g�p���āA�C���^�[�t�F�C�X�̌^�����擾�ł��܂��B
        /// </summary>  
        /// <param name="iTInfo">�Ԃ����^���B</param>
        /// <param name="lcid">�^���̃��P�[�� ID�B</param>
        /// <param name="info">�v�����ꂽ�^���I�u�W�F�N�g�ւ̃|�C���^�[�B</param>
        [PreserveSig]
        void GetTypeInfo(uint iTInfo, int lcid, out IntPtr info);

        /// <summary>
        /// ��A�̖��O��Ή������A�̃f�B�X�p�b�`���ʎq�Ɋ��蓖�Ă܂��B
        /// </summary>
        /// <param name="iid">�����g�p���邽�߂ɗ\�񂳂�Ă��܂��B Guid.Empty �ɂ���K�v������܂��B</param>
        /// <param name="names">�}�b�s���O�ΏۂƂ��ēn����閼�O�̔z��B</param>
        /// <param name="cNames">�}�b�s���O����閼�O�̃J�E���g�B</param>
        /// <param name="lcid">���O�����߂��郍�P�[�� �R���e�L�X�g�B</param>
        /// <param name="rgDispId">���O�ɑΉ����� ID ���󂯎��A�Ăяo���������蓖�Ă��z��B</param>
        [PreserveSig]
        void GetIDsOfNames(
            ref Guid iid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)]
                string[] names,
            uint cNames,
            int lcid,
            [Out]
                [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 2)]
                int[] rgDispId);

        /// <summary>
        /// �I�u�W�F�N�g�ɂ���Č��J���ꂽ�v���p�e�B����у��\�b�h�ւ̃A�N�Z�X��񋟂��܂��B
        /// </summary>
        /// <param name="dispIdMember">�����o�[�����ʂ��܂��B</param>
        /// <param name="riid">�����g�p���邽�߂ɗ\�񂳂�Ă��܂��B Guid.Empty �ɂ���K�v������܂��B</param>
        /// <param name="lcid">���������߂���Ώۂ̃��P�[�� �R���e�L�X�g�B</param>
        /// <param name="wFlags">�Ăяo���̃R���e�L�X�g���L�q����t���O�B</param>
        /// <param name="pDispParams">�����̔z��A���O�t�������� DISPID �̔z��A�z����̗v�f���̃J�E���g���i�[���Ă���\���̂ւ̃|�C���^�[�B</param>
        /// <param name="pvarResult">���ʂ��i�[�����ꏊ�ւ̃|�C���^�[�B</param>
        /// <param name="pExcepInfo">��O�����i�[����\���̂ւ̃|�C���^�[�B</param>
        /// <param name="puArgErr">�G���[�����݂���ŏ��̈����̃C���f�b�N�X�B</param>
        [PreserveSig]
        void Invoke(
            int dispIdMember,
            ref Guid riid,
            int lcid,
            ComTypes.INVOKEKIND wFlags,
            ref ComTypes.DISPPARAMS pDispParams,
            IntPtr pvarResult,
            IntPtr pExcepInfo,
            IntPtr puArgErr);
    }

}
