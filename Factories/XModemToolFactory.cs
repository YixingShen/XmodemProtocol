﻿namespace XModemProtocol.Factories {
    using Tools;
    using Builders;
    using Calculators;
    using Validators.Packet;
    using Validators.Checksum;
    public class XModemToolFactory : IToolFactory {
        /// <summary>
        /// Get the tools needed for a mode.
        /// </summary>
        /// <param name="mode">The mode for which tools are requested.</param>
        /// <returns>The tools.</returns>
        public IXModemTools GetToolsFor(XModemMode mode) {
            switch(mode) {
                case XModemMode.Checksum:
                    return _checksumTool;
                case XModemMode.CRC:
                    return _CRCTool;
                case XModemMode.OneK:
                    return _oneKTool;
                default:
                    return null;
            }
        }

        XModemTool _checksumTool;
        XModemTool _CRCTool;
        XModemTool _oneKTool;

        public int Polynomial {
            get { return _table.Polynomial; }
            set {
                if (_table.Polynomial == value) return;
                _table = new LookUpTable(value);
                TableChanged();
            }
        }

        LookUpTable _table = new LookUpTable(0x1021);
        NormalChecksumCalculator _calculator = new NormalChecksumCalculator();

        NormalChecksumValidator _normalChecksumValidator;
        PacketValidator _validator;
        NormalPacketBuilder _normalPacketBuilder;

        PacketValidator _CRCvalidator;
        CRCChecksumCalculator _cRCCalculator;
        CRCChecksumValidator _crcChecksumValidator;

        CRCPacketBuilder _CRCPacketBuilder;
        OneKPacketBuilder _oneKPacketBuilder;

        public XModemToolFactory() {
            _normalChecksumValidator = new NormalChecksumValidator(_calculator);
            _validator = new PacketValidator(_normalChecksumValidator);
            _normalPacketBuilder = new NormalPacketBuilder(_calculator);

            _CRCTool = new XModemTool {
                Builder = _CRCPacketBuilder,
                Validator = _CRCvalidator
            };
            TableChanged();
        }

        void TableChanged() {
            _cRCCalculator = new CRCChecksumCalculator(_table);
            _crcChecksumValidator = new CRCChecksumValidator(_cRCCalculator);
            _CRCvalidator = new PacketValidator(_crcChecksumValidator); 

            _CRCPacketBuilder = new CRCPacketBuilder(_cRCCalculator);
            _oneKPacketBuilder = new OneKPacketBuilder(_cRCCalculator);

            _oneKTool = new XModemTool {
                Builder = _oneKPacketBuilder,
                Validator = _CRCvalidator
            };

            _checksumTool = new XModemTool {
                Builder = _normalPacketBuilder,
                Validator = _validator
            };
        }

    }
}